using Microsoft.Extensions.Localization;
using Promasy.Application.Interfaces;
using Promasy.Core.Constants;
using Promasy.Core.Exceptions;
using Promasy.Domain.Employees;
using Promasy.Domain.Orders;
using Promasy.Modules.Orders.Dtos;
using Promasy.Modules.Orders.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Unit = QuestPDF.Infrastructure.Unit;

namespace Promasy.Modules.Orders.Services;

internal class OrderExporter : IOrderExporter
{
    private static readonly TextStyle StyleLarge = TextStyle.Default.FontSize(14).FontFamily(Fonts.Arial);
    private static readonly TextStyle StyleMedium = TextStyle.Default.FontSize(12).FontFamily(Fonts.Arial);
    private static readonly TextStyle StyleSmall = TextStyle.Default.FontSize(10).FontFamily(Fonts.Arial);
    
    private readonly IStringLocalizer<OrderExporter> _localizer;
    private readonly IStringLocalizer<RoleName> _roleLocalizer;
    private readonly IStringLocalizer<OrderType> _orderTypeLocalizer;
    private readonly IFileStorage _fileStorage;
    private readonly IOrderGroupRepository _orderGroupRepository;
    private readonly IEmployeesRepository _employeesRepository;
    private readonly IOrdersRepository _ordersRepository;
    private readonly IUserContext _context;

    public OrderExporter(IStringLocalizer<OrderExporter> localizer, IStringLocalizer<RoleName> roleLocalizer,
        IUserContext context, IStringLocalizer<OrderType> orderTypeLocalizer, IFileStorage fileStorage,
        IOrderGroupRepository orderGroupRepository, IEmployeesRepository employeesRepository, IOrdersRepository ordersRepository)
    {
        _localizer = localizer;
        _roleLocalizer = roleLocalizer;
        _context = context;
        _orderTypeLocalizer = orderTypeLocalizer;
        _fileStorage = fileStorage;
        _orderGroupRepository = orderGroupRepository;
        _employeesRepository = employeesRepository;
        _ordersRepository = ordersRepository;
    }
    
    public async Task ExportToPdfFileAsync(string fileKey)
    {
        var group = await _orderGroupRepository.GetOrderGroupByKeyAsync(fileKey);
        if (group is null || group.Status != OrderGroupStatus.Created)
        {
            return;
        }
        
        var roles = group.Employees.Select(e => (e.EmployeeId, e.Role))
            .Concat(new [] { (group.EditorId, RoleName.User) });
        
        var employees = await _employeesRepository.GetByIdsAndRolesAsync(roles);
        var orders = await _ordersRepository.GetByIdsAsync(group.OrderIds);
        foreach (var departmentId in orders.Select(o => o.Department.Id).Distinct())
        {
            var departmentHeads = await _employeesRepository.GetEmployeesForDepartmentIdAsync(departmentId,
                RoleName.HeadOfDepartment, RoleName.PersonallyLiableEmployee);
            employees.AddRange(departmentHeads);
        }

        GeneratePdf(group.FileKey, orders, employees);
        
        await _orderGroupRepository.SetGroupStatusAsync(group.FileKey, OrderGroupStatus.FileGenerated);
    }

    private void GeneratePdf(string fileName, ICollection<OrderDto> orders, ICollection<EmployeeDto> employees)
    {
        var headEmployee =
            employees.FirstOrDefault(e => e.Role is RoleName.Director or RoleName.DeputyDirector);
        if (headEmployee is null)
        {
            throw new ServiceException(string.Format(_localizer["Employee with required Role {0} is missing"],
                $"{_roleLocalizer[RoleName.Director.ToString()]}/{_roleLocalizer[RoleName.DeputyDirector.ToString()]}"));
        }
        
        var customer = employees.FirstOrDefault(e => e.Role is RoleName.User);
        if (customer is null)
        {
            throw new ServiceException(string.Format(_localizer["Employee with required Role {0} is missing"],
                _roleLocalizer[RoleName.User.ToString()]));
        }
        
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.PageColor(Colors.White);

                page.Header()
                    .ShowOnce()
                    .Element(c => ComposeHeader(c, headEmployee));

                page.Content()
                    .Element(c => ComposeContent(c, orders, employees));

                page.Footer()
                    .Element(c => ComposeFooter(c, customer));
            });
        });

        document.GeneratePdf(_fileStorage.GetPathForFile(fileName));
    }

    private void ComposeHeader(IContainer container, EmployeeDto employee)
    {
        container.Row(row =>
        {
            row.RelativeItem().AlignRight()
                .PaddingRight(8, Unit.Millimetre)
                .PaddingTop(8, Unit.Millimetre)
                .Column(column =>
                {
                    column.Item().AlignCenter().Text(_localizer["Approved"]).Style(StyleLarge).SemiBold();

                    column.Item().AlignCenter().Text(_roleLocalizer[employee.Role.ToString()]).Style(StyleLarge);

                    column.Item().Text(text =>
                    {
                        text.Span(employee.Name).Style(StyleLarge);
                        text.Span(" _______________").Style(StyleLarge);
                    });
                });
        });
    }

    private void ComposeFooter(IContainer container, EmployeeDto employee)
    {
        container.Row(row =>
        {
            row.RelativeItem()
                .PaddingLeft(8, Unit.Millimetre)
                .Text(x =>
                {
                    x.Span($"{_localizer["Generated on"]}: ").Style(StyleSmall);
                    x.Span(_context.AsUserTime(DateTime.UtcNow).ToString("G")).Style(StyleSmall);
                    x.Span($". {_localizer["Customer"]}: ").Style(StyleSmall);
                    x.Span(employee.Name).Style(StyleSmall);
                    x.Span($" {_localizer["Phone"]}: ").Style(StyleSmall);
                    x.Span(employee.Phone).Style(StyleSmall);
                });

            row.RelativeItem().AlignRight()
                .PaddingBottom(8, Unit.Millimetre)
                .PaddingRight(8, Unit.Millimetre)
                .Text(x =>
                {
                    x.Span($"{_localizer["Page"]} ").Style(StyleSmall);
                    x.CurrentPageNumber().Style(StyleSmall);
                    x.Span($" {_localizer["of"]} ").Style(StyleSmall);
                    x.TotalPages().Style(StyleSmall);
                });
        });
    }

    private void ComposeContent(IContainer container, ICollection<OrderDto> orders, ICollection<EmployeeDto> employees)
    {
        container
            .PaddingTop(5, Unit.Millimetre)
            .PaddingBottom(2, Unit.Millimetre)
            .PaddingHorizontal(8, Unit.Millimetre)
            .Shrink()
            .Border(0)
            .Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(70);
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.ConstantColumn(60);
                    columns.ConstantColumn(60);
                    columns.ConstantColumn(100);
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.ConstantColumn(70);
                });

                table.ExtendLastCellsToTableBottom();

                foreach (var (typeGroup, typeGroupIndex) in orders.GroupBy(o => o.Type).Select((g, i) => (g, i)))
                {
                    if (typeGroupIndex > 0)
                    {
                        table.Cell().PageBreak();
                    }

                    table.Cell().ColumnSpan(9)
                        .LabelCell(Colors.Grey.Lighten5)
                        .AlignCenter()
                        .Text(text =>
                        {
                            text.Span($"{_localizer["Orders of type"]} '").Style(StyleMedium);
                            text.Span(_orderTypeLocalizer[typeGroup.Key.ToString()]).Style(StyleMedium).Bold();
                            text.Span("'").Style(StyleMedium);
                        });

                    foreach (var (departmentGroup, departmentGroupIndex) in typeGroup.GroupBy(o => o.Department).Select((g, i) => (g, i)))
                    {
                        if (departmentGroupIndex > 0)
                        {
                            table.Cell().PageBreak();
                        }
                        
                        table.Cell().ColumnSpan(9)
                            .LabelCell(Colors.Grey.Lighten4)
                            .PaddingLeft(6, Unit.Millimetre)
                            .Text(departmentGroup.Key.Name)
                            .Style(StyleMedium)
                            .Bold();

                        var subDepartmentsCount = departmentGroup.Select(o => o.SubDepartment.Id).Distinct().Count();
                        foreach (var subDepartmentGroup in departmentGroup.GroupBy(o => o.SubDepartment))
                        {
                            table.Cell().ColumnSpan(9)
                                .LabelCell(Colors.Grey.Lighten4)
                                .PaddingLeft(12, Unit.Millimetre)
                                .Text(text =>
                                {
                                    text.Span($"{_localizer["Sub-department"]} '").Style(StyleMedium);
                                    text.Span(subDepartmentGroup.Key.Name).Style(StyleMedium).Bold();
                                    text.Span("'").Style(StyleMedium);
                                });

                            var fundsCount = subDepartmentGroup.Select(o => o.FinanceSubDepartment.Id).Distinct().Count();
                            foreach (var financeSubDepartmentGroup in subDepartmentGroup.GroupBy(o =>
                                         o.FinanceSubDepartment))
                            {
                                table.Cell().ColumnSpan(9)
                                    .LabelCell(Colors.Grey.Lighten3)
                                    .PaddingLeft(18, Unit.Millimetre)
                                    .Text(text =>
                                    {
                                        text.Span($"{_localizer["Fund"]}: ").Style(StyleMedium);
                                        text.Span(
                                                $"{financeSubDepartmentGroup.Key.FinanceSourceNumber} - {financeSubDepartmentGroup.Key.FinanceSource}")
                                            .Style(StyleMedium).Bold();
                                    });
                                
                                DrawHeader(table);

                                foreach (var order in financeSubDepartmentGroup)
                                {
                                    DrawValueRow(table, order);
                                }

                                if (fundsCount > 1)
                                {
                                    DrawTotalsCell(table, _localizer["Total by fund"], financeSubDepartmentGroup.Sum(o => o.OnePrice * o.Amount));
                                }
                            }

                            if (subDepartmentsCount > 1)
                            {
                                DrawTotalsCell(table, _localizer["Total by sub-department"],
                                    subDepartmentGroup.Sum(o => o.OnePrice * o.Amount));
                            }
                        }
                        
                        DrawTotalsCell(table, _localizer["Total by department"],
                            departmentGroup.Sum(o => o.OnePrice * o.Amount));
                        
                        DrawSignatureCell(table, _roleLocalizer[RoleName.HeadOfDepartment.ToString()], employees.FirstOrDefault(e => e.DepartmentId == departmentGroup.Key.Id && e.Role == RoleName.HeadOfDepartment)?.Name, 5);
                        DrawSignatureCell(table, _roleLocalizer[RoleName.PersonallyLiableEmployee.ToString()], employees.FirstOrDefault(e => e.DepartmentId == departmentGroup.Key.Id && e.Role == RoleName.PersonallyLiableEmployee)?.Name, paddingBottom: 2);
                    }
                    
                    DrawTotalsCell(table, _localizer["Total"], orders.Sum(o => o.OnePrice * o.Amount));

                    DrawSignatureCell(table, _roleLocalizer[RoleName.ChiefAccountant.ToString()], employees.FirstOrDefault(e => e.Role == RoleName.ChiefAccountant)?.Name, 5);
                    DrawSignatureCell(table, _roleLocalizer[RoleName.ChiefEconomist.ToString()], employees.FirstOrDefault(e => e.Role == RoleName.ChiefEconomist)?.Name);
                    DrawSignatureCell(table, _roleLocalizer[RoleName.SecretaryOfTenderCommittee.ToString()], employees.FirstOrDefault(e => e.Role == RoleName.SecretaryOfTenderCommittee)?.Name, paddingBottom: 2);
                }
            });
    }

    private void DrawHeader(TableDescriptor table)
    {
        table.Cell().ColumnSpan(2)
            .LabelCell()
            .AlignCenter()
            .Text(_localizer["Classifier"])
            .Style(StyleMedium);

        table.Cell()
            .RowSpan(2)
            .LabelCell()
            .AlignCenter()
            .Text(_localizer["Order description"])
            .Style(StyleMedium);

        table.Cell().ColumnSpan(3)
            .LabelCell()
            .AlignCenter()
            .Text(_localizer["Ordered"])
            .Style(StyleMedium);

        table.Cell()
            .RowSpan(2)
            .LabelCell()
            .AlignCenter()
            .Text(_localizer["Possible producer and cat no"])
            .Style(StyleMedium);

        table.Cell()
            .RowSpan(2)
            .LabelCell()
            .AlignCenter()
            .Text(_localizer["Possible supplier"])
            .Style(StyleMedium);

        table.Cell()
            .RowSpan(2)
            .LabelCell()
            .AlignCenter()
            .Text(_localizer["Date"])
            .Style(StyleMedium);

        table.Cell()
            .LabelCell()
            .AlignCenter()
            .Text(_localizer["Code"])
            .Style(StyleMedium);

        table.Cell()
            .LabelCell()
            .AlignCenter()
            .Text(_localizer["Name"])
            .Style(StyleMedium);

        table.Cell()
            .LabelCell()
            .AlignCenter()
            .Text(_localizer["Amount"])
            .Style(StyleMedium);

        table.Cell()
            .LabelCell()
            .AlignCenter()
            .Text(_localizer["Price of unit"])
            .Style(StyleMedium);

        table.Cell()
            .LabelCell()
            .AlignCenter()
            .Text(_localizer["Total"])
            .Style(StyleMedium);
    }

    private void DrawValueRow(TableDescriptor table, OrderDto order)
    {
        table.Cell().ValueCell().Text(order.Cpv.Code).Style(StyleSmall);
        table.Cell().ValueCell().Text(_context.GetLocalizationCulture() == LocalizationCulture.Ukrainian ? order.Cpv.DescriptionUkrainian : order.Cpv.DescriptionEnglish).Style(StyleSmall);
        table.Cell().ValueCell().Text(order.Description).Style(StyleSmall);
        table.Cell().ValueCell().Text(text =>
        {
            text.Span(order.Amount.ToString("G29")).Style(StyleSmall);
            text.Span(" ").Style(StyleSmall);
            text.Span(order.Unit.Name).Style(StyleSmall);
        });
        table.Cell().ValueCell().AlignRight().Text(order.OnePrice.ToString("N2")).Style(StyleSmall);
        table.Cell().ValueCell().AlignRight().Text(order.Total.ToString("N2")).Style(StyleSmall);
        table.Cell().ValueCell().Text(text =>
        {
            text.Span(order.Manufacturer?.Name ?? _localizer["Any"]).Style(StyleSmall);
            if (order.CatNum is null) return;
            text.Span(" ").Style(StyleSmall);
            text.Span(order.CatNum).Style(StyleSmall);
        });
        table.Cell().ValueCell().Text(text =>
        {
            text.Span(order.Supplier?.Name ?? _localizer["Any"]).Style(StyleSmall);
            if (order.Reason is null) return;
            text.Span($" {_localizer["Reason"]}: ").Style(StyleSmall);
            text.Span(order.Reason.Name).Style(StyleSmall);
        });
        table.Cell().ValueCell().AlignCenter().Text(_context.AsUserTime(order.EditedDate).ToString("g")).Style(StyleSmall);
    }


    private static void DrawSignatureCell(TableDescriptor table, string title, string? fullName, int paddingTop = 2, int paddingBottom = 0)
    {
        const string signaturePlaceholder = "_____________";
        
        table.Cell().ColumnSpan(6)
            .PaddingTop(paddingTop, Unit.Millimetre)
            .PaddingRight(2, Unit.Millimetre)
            .PaddingBottom(paddingBottom, Unit.Millimetre)
            .AlignRight()
            .Text(title)
            .Style(StyleLarge);

        table.Cell()
            .PaddingTop(paddingTop, Unit.Millimetre)
            .PaddingBottom(paddingBottom, Unit.Millimetre)
            .Text(signaturePlaceholder)
            .Style(StyleLarge);

        table.Cell().ColumnSpan(2)
            .PaddingTop(paddingTop, Unit.Millimetre)
            .PaddingLeft(2, Unit.Millimetre)
            .PaddingBottom(paddingBottom, Unit.Millimetre)
            .Text(fullName)
            .Style(StyleLarge);
    }
    
    private static void DrawTotalsCell(TableDescriptor table, string name, decimal sum)
    {
        table.Cell()
            .ColumnSpan(7)
            .ValueCell()
            .AlignRight()
            .Text(name).Style(StyleMedium).Bold();
        
        table.Cell()
            .ColumnSpan(2)
            .ValueCell()
            .AlignRight()
            .Text(sum.ToString("c")).Style(StyleMedium).Bold();
    }
}

internal static class PdfContainerExtensions
{
    private static IContainer Cell(this IContainer container, string color)
    {
        return container
            .Border(1)
            .Background(color);
    }

// displays only text label
    public static IContainer LabelCell(this IContainer container, string? color = null) =>
        container.Cell(color ?? Colors.Grey.Lighten2);

// allows to inject any type of content, e.g. image
    public static IContainer ValueCell(this IContainer container, string? color = null) => 
        container.Cell(color ?? Colors.White)
        .PaddingHorizontal(1, Unit.Millimetre);
}