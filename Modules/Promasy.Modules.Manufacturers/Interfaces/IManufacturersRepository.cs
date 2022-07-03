﻿using Promasy.Modules.Core.Modules;
using Promasy.Modules.Core.Requests;
using Promasy.Modules.Core.Responses;
using Promasy.Modules.Manufacturers.Dtos;

namespace Promasy.Modules.Manufacturers.Interfaces;

internal interface IManufacturersRepository : IRepository
{
    Task<PagedResponse<ManufacturerDto>> GetPagedListAsync(PagedRequest request);
    Task<ManufacturerDto?> GetByIdAsync(int id);
    Task DeleteByIdAsync(int id);
    Task<int> CreateAsync(ManufacturerDto unit);
    Task UpdateAsync(ManufacturerDto unit);
    Task MergeAsync(int targetId, int[] sourceIds);
}