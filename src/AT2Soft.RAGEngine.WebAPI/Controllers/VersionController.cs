using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace AT2Soft.RAGEngine.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class VersionController : ControllerBase
{
    public sealed record AssamblyRef (string Name, string Version);

    [HttpGet(Name = "GetVersion")]
    public  ActionResult GetVersion([FromQuery]bool all = false)
    {
        List<AssamblyRef> result = new();

        // Obtener el ensamblado actual
        var currentAssembly = Assembly.GetExecutingAssembly();
        var version = currentAssembly.GetName().Version?.ToString();

        result.Add(new(currentAssembly.GetName().Name ?? "Unknow", version ?? "Sin Version"));


        // Obtener todos los ensamblados referenciados
        var referencedAssemblies = currentAssembly.GetReferencedAssemblies();
        foreach (var assemblyName in referencedAssemblies)
        {
            // Cargar cada ensamblado referenciado para obtener detalles
            var loadedAssembly = Assembly.Load(assemblyName);
            var ver = loadedAssembly.GetName().Version?.ToString();
            var aName = assemblyName.Name ?? "Unknow";

            if (all | !aName.ToLower().StartsWith("microsoft.") | !aName.ToLower().StartsWith("system."))
                result.Add(new (aName, ver ?? "Sin Version"));
        }

        return Ok(result.OrderBy(x => x.Name));
    }
}