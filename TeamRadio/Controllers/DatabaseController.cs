using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace TeamRadio.Controllers;

public class DatabaseController(IConfiguration configuration) : Controller
{
    private readonly IConfiguration _configuration = configuration;

    // GET
    public async Task<IActionResult> Index()
    {
        var _connectionString = _configuration.GetConnectionString("Postgres");
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        // Perform a simple query
        await using var command = new NpgsqlCommand("SELECT 1", connection);
        var result = await command.ExecuteScalarAsync();

        return View(result != null && (int)result == 1);
    }
}