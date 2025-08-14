using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TutorConnectDocker.Models;

[Index("Email", Name = "UQ__Students__A9D105347A7950F6", IsUnique = true)]
public partial class Students
{
    [Key]
    public int StudentId { get; set; }

    [StringLength(100)]
    public string FullName { get; set; } = null!;

    [StringLength(100)]
    public string Email { get; set; } = null!;

    [StringLength(15)]
    public string? PhoneNumber { get; set; }

    [InverseProperty("Student")]
    public virtual ICollection<Sessions> Sessions { get; set; } = new List<Sessions>();
}
