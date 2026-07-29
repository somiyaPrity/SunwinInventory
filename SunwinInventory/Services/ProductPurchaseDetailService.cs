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
                CostPricePerPieceTk = x.CostPricePerPieceTk,

                CostPriceKgRmb = x.CostPriceKgRmb,
                CostPriceLbsRmb = x.CostPriceLbsRmb,
                CostPricePcsRmb = null, // Model doesn't contain this property

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
                CostPricePerPieceTk = x.CostPricePerPieceTk,

                CostPriceKgRmb = x.CostPriceKgRmb,
                CostPriceLbsRmb = x.CostPriceLbsRmb,
                CostPricePcsRmb = null,

                TotalAmountWithoutTaxTk = x.TotalAmountWithoutTaxTk,
                TotalAmountIncludingTaxTk = x.TotalAmountIncludingTaxTk,

                TotalAmountWithoutTaxRmb = x.TotalAmountWithoutTaxRmb,
                TotalAmountIncludingTaxRmb = x.TotalAmountIncludingTaxRmb,

                Remarks = x.Remarks
            };
        }

        public async Task CreateAsync(ProductPurchaseDetail entity)
        {
            entity.CreatedBy =
                _httpContextAccessor.HttpContext?
                .Session.GetString("UserName") ?? "System";

            entity.CreatedDate = DateTime.Now;

            await _repository.CreateAsync(entity);
        }

        public async Task UpdateAsync(ProductPurchaseDetail entity)
        {
            entity.UpdatedBy =
                _httpContextAccessor.HttpContext?
                .Session.GetString("UserName") ?? "System";

            entity.UpdatedDate = DateTime.Now;

            await _repository.UpdateAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}