using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TinyUrlBackend.Data; // Ensure this matches your namespace for ApplicationDbContext
using TinyUrlBackend.Models; // Ensure this matches your namespace for the Url model

namespace TinyUrlBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TinyUrlController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TinyUrlController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/tinyurl
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TinyUrlAddDto dto)
        {
            var code = Guid.NewGuid().ToString().Substring(0, 6);

            var newUrl = new TinyUrl
            {
                code = code,
                originalURL = dto.originalURL,
                shortURL = $"https://app-tinyurl-6f730274.azurewebsites.net/{code}",
                isPrivate = dto.isPrivate,
                totalClicks = 0
            };

            _context.Urls.Add(newUrl);
            await _context.SaveChangesAsync();

            return Ok(newUrl);
        }

        // GET: api/tinyurl/{code}
        [HttpGet("{code}")]
        public async Task<IActionResult> RedirectTo(string code)
        {
            var mapping = await _context.Urls.FirstOrDefaultAsync(u => u.code == code);
            if (mapping == null) return NotFound();

            return Redirect(mapping.originalURL);
        }
    }
}