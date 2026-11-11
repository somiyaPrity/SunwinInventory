using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SunwinInventory.Models;

public partial class Shipment
{
    [Key]
    public int ShipmentId { get; set; }

    [StringLength(30)]
    public string ShipmentNo { get; set; } = null!;

    public DateOnly ShipmentDate { get; set; }

    [StringLength(50)]
    public string? ShipmentType { get; set; }

    [StringLength(100)]
    public string? VesselName { get; set; }

    [StringLength(100)]
    public string? ContainerNo { get; set; }

    [Column("BLNo")]
    [StringLength(100)]
    public string? Blno { get; set; }

    public DateOnly? ArrivalDate { get; set; }

    [StringLength(500)]
    public string? Remarks { get; set; }

    public bool IsActive { get; set; }

    [StringLength(100)]
    public string CreatedBy { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime CreatedDate { get; set; }

    [StringLength(100)]
    public string? UpdatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdatedDate { get; set; }
}
