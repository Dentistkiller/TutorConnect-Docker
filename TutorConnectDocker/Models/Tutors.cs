using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TutorConnectDocker.Models;

[Index("Email", Name = "UQ__Tutors__A9D10534EE651CDA", IsUnique = true)]
public partial class Tutors
{
    [Key]
    public int TutorId { get; set; }

    [StringLength(100)]
    public string FullName { get; set; } = null!;

    [StringLength(100)]
    public string Email { get; set; } = null!;

    [StringLength(50)]
    public string Subject { get; set; } = null!;

    [InverseProperty("Tutor")]
    public virtual ICollection<Sessions> Sessions { get; set; } = new List<Sessions>();
}
