using Microsoft.AspNetCore.Http;
using SunwinInventory.DTOs.ProductPurchaseDetail;
using SunwinInventory.Models;
using SunwinInventory.Repositories.Interfaces;
using SunwinInventory.Services.Interfaces;

namespace SunwinInventory.Services
{
    public class ProductPurchaseDetailService : IProductPurchaseDetailService
    {
        private readonly IProductPurchaseDetailRepository _repository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProductPurchaseDetailService(
            IProductPurchaseDetailRepository repository,
            IHttpContextAccessor httpContextAccessor)
        {
            _repository = repository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<ProductPurchaseDetailDto>> GetAllAsync()
        {
            var list = await _repository.GetAllAsync();

            return list.Select(x => new ProductPurchaseDetailDto
            {
                PurchaseDetailId = x.PurchaseDetailId,
                ShipmentId = x.ShipmentId,
                SupplierId = x.SupplierId,
                WarehouseId = x.WarehouseId,
                ProductNameId = x.ProductNameId,
                ColorId = x.ColorId,
                Thickness = x.Thickness,
                ProductSize = x.ProductSize,
                InitialQuantity = x.InitialQuantity,
                UnitWeightKg = x.UnitWeightKg,
                TotalWeightKg = x.TotalWeightKg,
                ExchangeRate = x.ExchangeRate,
                UnitPriceRmb = x.UnitPriceRmb,
                UnitPriceTk = x.UnitPriceTk,
                ImportTaxPerKgTk = x.ImportTaxPerKgTk,
                ImportTaxPerKgRmb = x.ImportTaxPerKgRmb,
                CostPriceKgTk = x.CostPriceKgTk,
                CostPriceLbsTk = x.CostPriceLbsTk,
                CostPriceKgRmb = x.CostPriceKgRmb,
                CostPriceLbsRmb = x.CostPriceLbsRmb,
                TotalAmountWithoutTaxTk = x.TotalAmountWithoutTaxTk,
                TotalAmountIncludingTaxTk = x.TotalAmountIncludingTaxTk,
                TotalAmountWithoutTaxRmb = x.TotalAmountWithoutTaxRmb,
                TotalAmountIncludingTaxRmb = x.TotalAmountIncludingTaxRmb,
                Remarks = x.Remarks
            }).ToList();
        }

        public async Task<ProductPurchaseDetailDto?> GetByIdAsync(int id)
        {
            var x = await _repository.GetByIdAsync(id);

            if (x == null)
                return null;

            return new ProductPurchaseDetailDto
            {
                PurchaseDetailId = x.PurchaseDetailId,
                ShipmentId = x.ShipmentId,
                SupplierId = x.SupplierId,
                WarehouseId = x.WarehouseId,
                ProductNameId = x.ProductNameId,
                ColorId = x.ColorId,
                Thickness = x.Thickness,
                ProductSize = x.ProductSize,
                InitialQuantity = x.InitialQuantity,
                UnitWeightKg = x.UnitWeightKg,
                TotalWeightKg = x.TotalWeightKg,
                ExchangeRate = x.ExchangeRate,
                UnitPriceRmb = x.UnitPriceRmb,
                UnitPriceTk = x.UnitPriceTk,
                ImportTaxPerKgTk = x.ImportTaxPerKgTk,
                ImportTaxPerKgRmb = x.ImportTaxPerKgRmb,
                CostPriceKgTk = x.CostPriceKgTk,
                CostPriceLbsTk = x.CostPriceLbsTk,
                CostPriceKgRmb = x.CostPriceKgRmb,
                CostPriceLbsRmb = x.CostPriceLbsRmb,
                TotalAmountWithoutTaxTk = x.TotalAmountWithoutTaxTk,
                TotalAmountIncludingTaxTk = x.TotalAmountIncludingTaxTk,
                TotalAmountWithoutTaxRmb = x.TotalAmountWithoutTaxRmb,
                TotalAmountIncludingTaxRmb = x.TotalAmountIncludingTaxRmb,
                Remarks = x.Remarks
            };
        }
        public async Task CreateAsync(PurchaseEntryDto dto)
        {
            await _repository.CreateAsync(dto);
        }
        //public async Task CreateAsync(PurchaseEntryDto dto)
        //{
        //    var entities = new List<ProductPurchaseDetail>();

        //    foreach (var item in dto.Items)
        //    {
        //        entities.Add(new ProductPurchaseDetail
        //        {
        //            ShipmentId = dto.ShipmentId,
        //            SupplierId = item.SupplierId,
        //            WarehouseId = item.WarehouseId,
        //            ProductNameId = item.ProductNameId,
        //            ColorId = item.ColorId,
        //            Thickness = item.Thickness,
        //            ProductSize = item.ProductSize,
        //            InitialQuantity = item.InitialQuantity,
        //            UnitWeightKg = item.UnitWeightKg,
        //            TotalWeightKg = item.TotalWeightKg,
        //            ExchangeRate = item.ExchangeRate,
        //            UnitPriceRmb = item.UnitPriceRmb,
        //            UnitPriceTk = item.UnitPriceTk,
        //            ImportTaxPerKgTk = item.ImportTaxPerKgTk,
        //            ImportTaxPerKgRmb = item.ImportTaxPerKgRmb,
        //            CostPriceKgTk = item.CostPriceKgTk,
        //            CostPriceLbsTk = item.CostPriceLbsTk,
        //            CostPriceKgRmb = item.CostPriceKgRmb,
        //            CostPriceLbsRmb = item.CostPriceLbsRmb,
        //            TotalAmountWithoutTaxTk = item.TotalAmountWithoutTaxTk,
        //            TotalAmountIncludingTaxTk = item.TotalAmountIncludingTaxTk,
        //            TotalAmountWithoutTaxRmb = item.TotalAmountWithoutTaxRmb,
        //            TotalAmountIncludingTaxRmb = item.TotalAmountIncludingTaxRmb,
        //            Remarks = item.Remarks,
        //            CreatedBy = _httpContextAccessor.HttpContext?
        //                .Session.GetString("UserName") ?? "System",
        //            CreatedDate = DateTime.Now
        //        });
        //    }

        //    await _repository.AddRangeAsync(entities);
        //}

        public async Task UpdateAsync(ProductPurchaseDetailDto dto)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}