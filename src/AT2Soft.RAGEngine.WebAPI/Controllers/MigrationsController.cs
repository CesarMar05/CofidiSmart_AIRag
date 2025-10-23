using AT2Soft.RAGEngine.Application.Persistence.Migrations;
using Microsoft.AspNetCore.Mvc;

namespace AT2Soft.RAGEngine.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MigrationsController : ControllerBase
{
    private readonly IMigrationRepository _migrationRepository;

    public MigrationsController(IMigrationRepository migrationRepository)
    {
        _migrationRepository = migrationRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetStatus(CancellationToken cancellationToken)
    {
        try
        {
            var rslt = await _migrationRepository.ActualStatusAsync(cancellationToken);

            return string.IsNullOrWhiteSpace(rslt)
                ? StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    error = "Error al obtener el estado de la migración.",
                    details = "No se obtuvo respuesta del comando."
                })
                : Ok(rslt);
        }
        catch(Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                error = "Error al obtener el estado de la migración.",
                details = ex.Message
            });    
        }
    }

    [HttpGet("Update")]
    public async Task<IActionResult> UpdateDatabase(CancellationToken cancellationToken)
    {
        try
        {
            var rslt = await _migrationRepository.UpdateDatabaseAsync(cancellationToken);

            return string.IsNullOrWhiteSpace(rslt)
            ? StatusCode(StatusCodes.Status500InternalServerError, new
            {
                error = "Error al actualizar la base de datos.",
                details = "No se obtuvo respuesta del comando."
            })
            : Ok(rslt);
        }
        catch(Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                error = "Error al actualizar la base de datos.",
                details = ex.Message
            });
        }
    }

    //[HttpPost("AddAdminUser")]
    //public async Task<ActionResult> AddAdminUser(MigrationAddAdminUser adminUser, CancellationToken cancellationToken)
    //{
    //    var command = new MigrationAddAdminUserCommand(adminUser.CiaTaxId, adminUser.CiaName, adminUser.UserEmail, adminUser.UserName, adminUser.UserPhone);
    //    var result = await _sender.Send(command, cancellationToken);

    //    return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    //}

    //[HttpPost("UpdateUserCompany")]
    //public async Task<IActionResult> UpdateUserCompany(CancellationToken cancellationToken)
    //{
    //    try
    //    {
    //        var rslt = await _companyUserUpdateService.UpdateAsync(cancellationToken);
    //        return Ok(new { cambios = rslt });
    //    }
    //    catch (Exception ex)
    //    {
    //        return BadRequest(ex.Message);
    //    }
    //}
}

