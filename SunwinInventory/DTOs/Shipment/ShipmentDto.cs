using System.ComponentModel.DataAnnotations;

namespace SunwinInventory.DTOs.Shipment
{
    public class ShipmentDto
    {
        public int ShipmentId { get; set; }

        [Display(Name = "Shipment No")]
        public string ShipmentNo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Shipment Date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Shipment Date")]
        public DateTime ShipmentDate { get; set; } = DateTime.Today;

        [Display(Name = "Shipment Type")]
        public string? ShipmentType { get; set; }

        [Display(Name = "Vessel Name")]
        public string? VesselName { get; set; }

        [Display(Name = "Container No")]
        public string? ContainerNo { get; set; }

        [Display(Name = "BL No")]
        public string? BLNo { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Arrival Date")]
        public DateTime? ArrivalDate { get; set; }

        public string? Remarks { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime? CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }
    }
}