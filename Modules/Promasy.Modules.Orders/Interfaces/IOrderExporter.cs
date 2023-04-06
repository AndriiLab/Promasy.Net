namespace Promasy.Modules.Orders.Interfaces;

internal interface IOrderExporter
{
    Task ExportToPdfFileAsync(string fileKey);
}