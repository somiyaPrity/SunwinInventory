using System;
using System.Collections.Generic;
using System.Linq;

namespace SunwinInventory.Authorization
{
    public enum PermissionAction
    {
        View,
        Create,
        Edit,
        Delete
    }

    public sealed class MenuDefinition
    {
        public MenuDefinition(
            string key,
            string title,
            string group,
            string icon,
            string controller,
            string[] actions,
            string[]? createActions = null,
            string[]? editActions = null)
        {
            Key = key;
            Title = title;
            Group = group;
            Icon = icon;
            Controller = controller;
            Actions = actions;
            CreateActions = createActions ?? Array.Empty<string>();
            EditActions = editActions ?? Array.Empty<string>();
        }

        public string Key { get; }

        public string Title { get; }

        public string Group { get; }

        public string Icon { get; }

        public string Controller { get; }

        public string[] Actions { get; }

        public string[] CreateActions { get; }

        public string[] EditActions { get; }
    }

    // Single source of truth for menu labels, routes and permission keys.
    public static class MenuCatalog
    {
        public const string AdministratorRole = "Administrator";

        public static readonly IReadOnlyList<MenuDefinition> Menus =
            new List<MenuDefinition>
            {
                new("warehouse", "Warehouse", "MASTER SETUP", "bi-building",
                    "Warehouse",
                    new[] { "Index", "Create", "Edit", "Delete" },
                    new[] { "Create" }, new[] { "Edit" }),

                new("product-category", "Product Category",
                    "MASTER SETUP", "bi-tags",
                    "ProductCategory",
                    new[] { "Index", "Create", "Edit", "Delete" },
                    new[] { "Create" }, new[] { "Edit" }),

                new("product-name", "Product Name",
                    "MASTER SETUP", "bi-box-seam",
                    "ProductName",
                    new[] { "Index", "Create", "Edit", "Delete" },
                    new[] { "Create" }, new[] { "Edit" }),

                new("supplier", "Supplier", "MASTER SETUP", "bi-truck",
                    "Supplier",
                    new[] { "Index", "Create", "Edit", "Delete" },
                    new[] { "Create" }, new[] { "Edit" }),

                new("shipment", "Shipment", "MASTER SETUP", "bi-truck",
                    "Shipment",
                    new[] { "Index", "Create", "Edit", "Delete", "GetShipment" },
                    new[] { "Create" }, new[] { "Edit" }),

                new("purchase-entry", "Purchase Entry",
                    "MASTER SETUP", "bi-cart-plus",
                    "ProductPurchaseDetail",
                    new[]
                    {
                        "Create", "UploadExcel", "SaveExcelPurchase",
                        "Edit", "Delete"
                    },
                    new[] { "Create", "UploadExcel", "SaveExcelPurchase" },
                    new[] { "Edit" }),

                new("customers", "Customers", "Sale", "bi-people",
                    "Customer",
                    new[] { "Index", "Create", "Edit", "Delete" },
                    new[] { "Create" }, new[] { "Edit" }),

                new("sales-entry", "Sales Entry", "Sale",
                    "bi-arrow-left-right",
                    "Sales",
                    new[]
                    {
                        "Create", "GetStock", "GetProductSizes",
                        "GetStockOptions"
                    },
                    new[] { "Create" }),

                new("invoice-print", "Invoice Print", "Sale", "bi-printer",
                    "Sales",
                    new[] { "Index", "Invoice", "DownloadInvoicePdf" }),

                new("customer-due", "Receivables", "Sale",
                    "bi-hourglass-split",
                    "CustomerPayment",
                    new[]
                    {
                        "CustomerDue", "CustomerLedger",
                        "ExportReceivables", "DownloadCustomerStatement"
                    }),

                new("customer-due-payment", "Customer Due Payment List", "Sale",
                    "bi-cash-coin",
                    "CustomerPayment",
                    new[]
                    {
                        "Index", "Create", "PaymentReceipt",
                        "DownloadPaymentReceipt"
                    },
                    new[] { "Create" }),

                new("purchase-report", "Purchase Report", "REPORTS",
                    "bi-receipt",
                    "ProductPurchaseDetail",
                    new[] { "Index", "ExportExcel" }),

                new("sales-report", "Sales Report", "REPORTS", "bi-bar-chart",
                    "SalesReport",
                    new[] { "Index", "ExportExcel" }),

                new("sales-register", "Sales Register", "REPORTS",
                    "bi-journal-richtext",
                    "SalesReport",
                    new[] { "Register", "ExportRegisterExcel" }),

                new("stock-report", "Stock Report", "REPORTS",
                    "bi-clipboard-data",
                    "StockReport",
                    new[] { "Index", "ExportExcel" }),

                new("stock-movement", "Stock Movement", "REPORTS",
                    "bi-calendar3",
                    "StockReport",
                    new[] { "Movement", "ExportMovementExcel" }),

                new("fund-book", "Fund Book", "REPORTS",
                    "bi-journal-text",
                    "Fund",
                    new[]
                    {
                        "Index", "Create", "Edit", "Delete",
                        "ExportExcel", "OpeningBalances",
                        "SaveOpeningBalances"
                    },
                    new[] { "Create" },
                    new[] { "Edit", "SaveOpeningBalances" }),

                new("income-statement", "Income Statement", "REPORTS",
                    "bi-graph-up-arrow",
                    "FinancialReport",
                    new[] { "IncomeStatement", "ExportIncomeStatementExcel" }),

                new("balance-sheet", "Balance Sheet", "REPORTS",
                    "bi-bank",
                    "FinancialReport",
                    new[] { "BalanceSheet", "ExportBalanceSheetExcel" }),

                new("designation", "Designation", "Administrative",
                    "bi-person-badge",
                    "Designation",
                    new[] { "Index", "Create", "Edit", "Delete" },
                    new[] { "Create" }, new[] { "Edit" }),

                new("role", "Role", "Administrative", "bi-shield-lock",
                    "Role",
                    new[] { "Index", "Create", "Edit", "Delete" },
                    new[] { "Create" }, new[] { "Edit" }),

                new("users", "Users", "Administrative", "bi-people-fill",
                    "User",
                    new[] { "Index", "Create", "Edit", "Delete" },
                    new[] { "Create" }, new[] { "Edit" }),

                new("menu-access", "Menu Access", "Administrative",
                    "bi-ui-checks",
                    "MenuAccess",
                    new[] { "Index", "Save" },
                    null, new[] { "Save" })
            };

        public static IEnumerable<string> GroupOrder =>
            new[] { "MASTER SETUP", "Sale", "REPORTS", "Administrative" };

        public static MenuDefinition? Find(string key)
        {
            return Menus.FirstOrDefault(
                m => string.Equals(m.Key, key, StringComparison.OrdinalIgnoreCase));
        }

        public static bool TryResolve(
            string? controller,
            string? action,
            string httpMethod,
            out MenuDefinition menu,
            out PermissionAction required)
        {
            menu = null!;
            required = PermissionAction.View;

            if (string.IsNullOrWhiteSpace(controller) ||
                string.IsNullOrWhiteSpace(action))
            {
                return false;
            }

            var match = Menus.FirstOrDefault(
                m => string.Equals(m.Controller, controller,
                         StringComparison.OrdinalIgnoreCase) &&
                     m.Actions.Contains(action, StringComparer.OrdinalIgnoreCase));

            if (match == null)
                return false;

            menu = match;

            bool isPost = string.Equals(
                httpMethod, "POST", StringComparison.OrdinalIgnoreCase);

            if (!isPost)
                return true;

            if (string.Equals(action, "Delete", StringComparison.OrdinalIgnoreCase))
            {
                required = PermissionAction.Delete;
            }
            else if (match.CreateActions.Contains(
                         action, StringComparer.OrdinalIgnoreCase))
            {
                required = PermissionAction.Create;
            }
            else if (match.EditActions.Contains(
                         action, StringComparer.OrdinalIgnoreCase))
            {
                required = PermissionAction.Edit;
            }

            return true;
        }
    }
}
