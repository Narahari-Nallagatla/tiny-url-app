using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TinyUrlBackend.Models;

// This matches the "TinyUrl" schema in your image
public class TinyUrl
{
  
    public int Id { get; set; } // Database Primary Key   
    public string? code { get; set; }
    public string? shortURL { get; set; }
    public string? originalURL { get; set; }
    public int totalClicks { get; set; }
    public bool isPrivate { get; set; }
}

// This matches the "TinyUrlAddDto" schema in your image
public class TinyUrlAddDto
{
    public string? originalURL { get; set; }
    public bool isPrivate { get; set; }
}