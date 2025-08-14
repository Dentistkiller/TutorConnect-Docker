using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TutorConnectDocker.Models;

public partial class Sessions
{
    [Key]
    public int SessionId { get; set; }

    public int StudentId { get; set; }

    public int TutorId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime SessionDate { get; set; }

    public int DurationMinutes { get; set; }

    [ForeignKey("StudentId")]
    [InverseProperty("Sessions")]
    public virtual Students Student { get; set; } = null!;

    [ForeignKey("TutorId")]
    [InverseProperty("Sessions")]
    public virtual Tutors Tutor { get; set; } = null!;
}
