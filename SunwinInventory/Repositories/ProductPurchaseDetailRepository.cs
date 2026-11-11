using Microsoft.EntityFrameworkCore;
using SunwinInventory.Data;
using SunwinInventory.DTOs.ProductPurchaseDetail;
using SunwinInventory.Models;
using SunwinInventory.Repositories.Interfaces;

namespace SunwinInventory.Repositories
{
    public class ProductPurchaseDetailRepository : IProductPurchaseDetailRepository
    {
        private readonly SunwinInventoryContext _context;

        public ProductPurchaseDetailRepository(SunwinInventoryContext context)
        {
            _context = context;
        }

        public async Task<List<ProductPurchaseDetail>> GetAllAsync()
        {
            return await _context.ProductPurchaseDetails
                .OrderByDescending(x => x.PurchaseDetailId)
                .ToListAsync();
        }

        public async Task<ProductPurchaseDetail?> GetByIdAsync(int id)
        {
            return await _context.ProductPurchaseDetails
                .FirstOrDefaultAsync(x => x.PurchaseDetailId == id);
        }

        public async Task CreateAsync(ProductPurchaseDetail entity)
        {
            _context.ProductPurchaseDetails.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ProductPurchaseDetail entity)
        {
            _context.ProductPurchaseDetails.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.ProductPurchaseDetails.FindAsync(id);

            if (entity != null)
            {
                _context.ProductPurchaseDetails.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<List<PurchaseReportDto>> GetPurchaseReportAsync()
        {
            return await
            (
                from p in _context.ProductPurchaseDetails

                join s in _context.Shipments
                    on p.ShipmentId equals s.ShipmentId

                join sup in _context.Suppliers
                    on p.SupplierId equals sup.SupplierId

                join w in _context.Warehouses
                    on p.WarehouseId equals w.WarehouseId

                join pn in _context.ProductNames
                    on p.ProductNameId equals pn.ProductNameId into productGroup
                from pn in productGroup.DefaultIfEmpty()

                join c in _context.Colors
                    on p.ColorId equals c.ColorId into colorGroup
                from c in colorGroup.DefaultIfEmpty()

                orderby p.PurchaseDetailId descending

                select new PurchaseReportDto
                {
                    ProductPurchaseDetailId = p.PurchaseDetailId,

                    CreatedDate = p.CreatedDate,

                    ShipmentNo = s.ShipmentNo,

                    SupplierName = sup.SupplierName,

                    WarehouseName = w.WarehouseName,

                    ProductName = pn != null ? pn.Name : "",

                    ColorName = c != null ? c.ColorName : "",

                    Thickness = p.Thickness ?? 0,

                    ProductSize = p.ProductSize,

                    InitialQuantity = p.InitialQuantity,

                    UnitWeightKg = p.UnitWeightKg ?? 0,

                    TotalWeightKg = p.TotalWeightKg ?? 0,

                    ExchangeRate = p.ExchangeRate ?? 0,

                    UnitPriceTk = p.UnitPriceTk ?? 0,

                    UnitPriceRmb = p.UnitPriceRmb ?? 0,

                    ImportTaxPerKgTk = p.ImportTaxPerKgTk ?? 0,

                    ImportTaxPerKgRmb = p.ImportTaxPerKgRmb ?? 0,

                    CostPriceKgTk = p.CostPriceKgTk ?? 0,

                    CostPriceLbsTk = p.CostPriceLbsTk ?? 0,

                    CostPricePerPieceTk = p.CostPricePerPieceTk ?? 0,

                    CostPriceKgRmb = p.CostPriceKgRmb ?? 0,

                    CostPriceLbsRmb = p.CostPriceLbsRmb ?? 0,

                    CostPricePcsRmb = p.CostPricePerPieceRmb ?? 0,

                    TotalAmountWithoutTaxTk = p.TotalAmountWithoutTaxTk ?? 0,

                    TotalAmountIncludingTaxTk = p.TotalAmountIncludingTaxTk ?? 0,

                    TotalAmountWithoutTaxRmb = p.TotalAmountWithoutTaxRmb ?? 0,

                    TotalAmountIncludingTaxRmb = p.TotalAmountIncludingTaxRmb ?? 0,

                    Remarks = p.Remarks
                }
            ).ToListAsync();
        }
        public async Task<List<PurchaseReportDto>> GetPurchaseReportAsync(
    PurchaseReportFilterDto filter)
        {
            var query =
                from p in _context.ProductPurchaseDetails

                join s in _context.Shipments
                    on p.ShipmentId equals s.ShipmentId

                join sup in _context.Suppliers
                    on p.SupplierId equals sup.SupplierId

                join w in _context.Warehouses
                    on p.WarehouseId equals w.WarehouseId

                join pn in _context.ProductNames
                    on p.ProductNameId equals pn.ProductNameId

                join c1 in _context.Colors
                    on p.ColorId equals c1.ColorId into colorGroup

                from c in colorGroup.DefaultIfEmpty()

                select new
                {
                    p,
                    s,
                    sup,
                    w,
                    pn,
                    c
                };

            // =========================
            // FILTERS
            // =========================

            if (filter.FromDate.HasValue)
            {
                query = query.Where(x =>
                    x.p.CreatedDate.Date >= filter.FromDate.Value.Date);
            }

            if (filter.ToDate.HasValue)
            {
                query = query.Where(x =>
                    x.p.CreatedDate.Date <= filter.ToDate.Value.Date);
            }

            if (filter.SupplierId.HasValue)
            {
                query = query.Where(x =>
                    x.p.SupplierId == filter.SupplierId.Value);
            }

            if (filter.WarehouseId.HasValue)
            {
                query = query.Where(x =>
                    x.p.WarehouseId == filter.WarehouseId.Value);
            }

            if (filter.ShipmentId.HasValue)
            {
                query = query.Where(x =>
                    x.p.ShipmentId == filter.ShipmentId.Value);
            }

            if (filter.ProductNameId.HasValue)
            {
                query = query.Where(x =>
                    x.p.ProductNameId == filter.ProductNameId.Value);
            }

            // =========================
            // RESULT
            // =========================

            return await query
                .OrderByDescending(x => x.p.PurchaseDetailId)
                .Select(x => new PurchaseReportDto
                {
                    ProductPurchaseDetailId = x.p.PurchaseDetailId,

                    CreatedDate = x.p.CreatedDate,

                    ShipmentNo = x.s.ShipmentNo,

                    SupplierName = x.sup.SupplierName,

                    WarehouseName = x.w.WarehouseName,

                    ProductName = x.pn.Name,

                    ColorName = x.c == null
                        ? ""
                        : x.c.ColorName,

                    Thickness = x.p.Thickness ?? 0,

                    ProductSize = x.p.ProductSize,

                    // =========================
                    // QUANTITY / WEIGHT
                    // =========================

                    InitialQuantity =
                        x.p.InitialQuantity,

                    UnitWeightKg =
                        x.p.UnitWeightKg ?? 0,

                    TotalWeightKg =
                        x.p.TotalWeightKg ?? 0,

                    // =========================
                    // PRICE
                    // =========================

                    ExchangeRate =
                        x.p.ExchangeRate ?? 0,

                    UnitPriceTk =
                        x.p.UnitPriceTk ?? 0,

                    UnitPriceRmb =
                        x.p.UnitPriceRmb ?? 0,

                    // =========================
                    // IMPORT TAX
                    // =========================

                    ImportTaxPerKgTk =
                        x.p.ImportTaxPerKgTk ?? 0,

                    ImportTaxPerKgRmb =
                        x.p.ImportTaxPerKgRmb ?? 0,

                    // =========================
                    // COST PRICE
                    // =========================

                    CostPriceKgTk =
                        x.p.CostPriceKgTk ?? 0,

                    CostPriceLbsTk =
                        x.p.CostPriceLbsTk ?? 0,

                    CostPricePerPieceTk =
                        x.p.CostPricePerPieceTk ?? 0,

                    CostPriceKgRmb =
                        x.p.CostPriceKgRmb ?? 0,

                    CostPriceLbsRmb =
                        x.p.CostPriceLbsRmb ?? 0,

                    CostPricePcsRmb =
                        x.p.CostPricePerPieceRmb ?? 0,

                    // =========================
                    // TOTAL COST
                    // =========================

                    TotalAmountWithoutTaxTk =
                        x.p.TotalAmountWithoutTaxTk ?? 0,

                    TotalAmountIncludingTaxTk =
                        x.p.TotalAmountIncludingTaxTk ?? 0,

                    TotalAmountWithoutTaxRmb =
                        x.p.TotalAmountWithoutTaxRmb ?? 0,

                    TotalAmountIncludingTaxRmb =
                        x.p.TotalAmountIncludingTaxRmb ?? 0,

                    Remarks = x.p.Remarks
                })
                .ToListAsync();
        }
    }
}