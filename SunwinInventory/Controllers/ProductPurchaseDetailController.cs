using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SunwinInventory.DTOs.ProductPurchaseDetail;
using SunwinInventory.Services.Interfaces;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
namespace SunwinInventory.Controllers
{
    public class ProductPurchaseDetailController : Controller
    {
        private readonly IProductPurchaseDetailService _purchaseService;
        private readonly IShipmentService _shipmentService;
        private readonly ISupplierService _supplierService;
        private readonly IWarehouseService _warehouseService;
        private readonly IProductNameService _productNameService;
        private readonly ILookupService _lookupService;

        public ProductPurchaseDetailController(
            IProductPurchaseDetailService purchaseService,
            IShipmentService shipmentService,
            ISupplierService supplierService,
            IWarehouseService warehouseService,
            IProductNameService productNameService,
            ILookupService lookupService)
        {
            _purchaseService = purchaseService;
            _shipmentService = shipmentService;
            _supplierService = supplierService;
            _warehouseService = warehouseService;
            _productNameService = productNameService;
            _lookupService = lookupService; ;
        }


        [HttpPost]
        public async Task<IActionResult> UploadExcel(
         IFormFile file,
         int shipmentId,
         int supplierId,
         int warehouseId)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file selected.");

            var items = new List<ProductPurchaseDetailDto>();

            using var stream = file.OpenReadStream();
            using var workbook = new XLWorkbook(stream);

            var ws = workbook.Worksheet(1);

            int lastRow = ws.LastRowUsed().RowNumber();

            for (int row = 3; row <= lastRow; row++)
            {
                // Skip blank product rows
                if (string.IsNullOrWhiteSpace(ws.Cell(row, 3).GetString()))
                    continue;

                var dto = new ProductPurchaseDetailDto();

                // Header values
                dto.ShipmentId = shipmentId;
                dto.SupplierId = supplierId;
                dto.WarehouseId = warehouseId;

                // Excel values
                dto.ProductName = ws.Cell(row, 3).GetString().Trim();
                dto.ColorName = ws.Cell(row, 4).GetString().Trim();
                dto.Thickness = GetDecimal(ws, row, 5);
                dto.ProductSize = ws.Cell(row, 6).GetString().Trim();

                dto.InitialQuantity = (int)GetDecimal(ws, row, 7);
                dto.UnitWeightKg = GetDecimal(ws, row, 8);
                dto.TotalWeightKg = GetDecimal(ws, row, 9);

                dto.UnitPriceTk = GetDecimal(ws, row, 10);
                dto.ExchangeRate = GetDecimal(ws, row, 11);
                dto.UnitPriceRmb = GetDecimal(ws, row, 12);

                dto.ImportTaxPerKgTk = GetDecimal(ws, row, 13);

                dto.CostPriceKgTk = GetDecimal(ws, row, 14);
                dto.CostPriceLbsTk = GetDecimal(ws, row, 15);
                dto.CostPricePcsTk = GetDecimal(ws, row, 16);

                dto.TotalAmountWithoutTaxTk = GetDecimal(ws, row, 17);
                dto.TotalAmountWithoutTaxRmb = GetDecimal(ws, row, 18);

                dto.TotalAmountIncludingTaxTk = GetDecimal(ws, row, 19);
                dto.TotalAmountIncludingTaxRmb = GetDecimal(ws, row, 20);

                // Lookup IDs
                dto.ProductNameId = await _lookupService.GetProductIdAsync(dto.ProductName) ?? 0;
                dto.ColorId = await _lookupService.GetColorIdAsync(dto.ColorName) ?? 0;

                // Debug
                Console.WriteLine(
                    $"Row:{row} Product:[{dto.ProductName}] ProductId:{dto.ProductNameId} Color:[{dto.ColorName}] ColorId:{dto.ColorId}");

                items.Add(dto);
            }

            return Json(items);
        }
        private decimal GetDecimal(IXLWorksheet ws, int row, int column)
        {
            var value = ws.Cell(row, column).GetString()
                .Replace(",", "")
                .Trim();

            if (decimal.TryParse(value, out decimal result))
                return result;

            return 0;
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadDropDowns();

            return View(new PurchaseEntryDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PurchaseEntryDto dto)
        {
            foreach (var state in ModelState)
            {
                foreach (var error in state.Value.Errors)
                {
                    Console.WriteLine($"{state.Key} => {error.ErrorMessage}");
                }
            }
            if (!ModelState.IsValid)
            {
                await LoadDropDowns();
                return View(dto);
            }

            if (dto.Items == null || dto.Items.Count == 0)
            {
                TempData["Error"] = "Please add at least one product.";

                await LoadDropDowns();

                return View(dto);
            }

            foreach (var item in dto.Items)
            {
                item.ShipmentId = dto.ShipmentId;
            }

            try
            {
                await _purchaseService.CreateAsync(dto);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException?.Message);
                throw;
            }

            TempData["Success"] = "Purchase saved successfully.";

            return RedirectToAction(nameof(Create));
        }

        //[HttpPost]
        //public async Task<IActionResult> Create(PurchaseEntryDto dto)
        //{
        //    if (dto.Items == null || !dto.Items.Any())
        //    {
        //        TempData["Error"] = "Please add at least one product.";

        //        await LoadDropDowns();

        //        return View(dto);
        //    }

        //    await _purchaseService.CreateAsync(dto);

        //    TempData["Success"] = "Purchase saved successfully.";

        //    return RedirectToAction(nameof(Create));
        //}

        private async Task LoadDropDowns()
        {
            ViewBag.Shipments = new SelectList(
                await _shipmentService.GetAllAsync(),
                "ShipmentId",
                "ShipmentNo");

            ViewBag.Suppliers = new SelectList(
                await _supplierService.GetAllAsync(),
                "SupplierId",
                "SupplierName");

            ViewBag.Warehouses = new SelectList(
                await _warehouseService.GetAllAsync(),
                "WarehouseId",
                "WarehouseName");

            var products = await _productNameService.GetAllAsync();

            if (products == null)
            {
                throw new Exception("Products is NULL");
            }

            ViewBag.Products = new SelectList(
                products,
                "ProductNameId",
                "Name");

            ViewBag.Colors = await _lookupService.GetColorListAsync();
        }
    }
}