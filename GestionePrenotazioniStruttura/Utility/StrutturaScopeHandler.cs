using GestionePrenotazioniStruttura.Data;
using GestionePrenotazioniStruttura.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

public class StrutturaScopeHandler
    : AuthorizationHandler<StrutturaScopeRequirement, int>
{
    private readonly AppDbContext _ctx;

    public StrutturaScopeHandler(AppDbContext ctx)
    {
        _ctx = ctx;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        StrutturaScopeRequirement requirement,
        int resourceId)
    {
        if (context.User.IsInRole("Admin"))
        {
            context.Succeed(requirement);
            return;
        }

        if (!context.User.IsInRole("Revisor"))
            return;

        var userStructureId = int.Parse(
            context.User.FindFirst("StructureId")!.Value);

       
        int? structureId =
            await _ctx.Activities.Where(a => a.Id == resourceId).Select(a => (int?)a.StructureId).FirstOrDefaultAsync()
         ?? await _ctx.Rooms.Where(r => r.Id == resourceId).Select(r => (int?)r.StructureId).FirstOrDefaultAsync()
         ?? await _ctx.Bookings.Where(b => b.Id == resourceId).Select(b => (int?)b.Room.StructureId).FirstOrDefaultAsync()
         ?? await _ctx.Subscriptions
                .Where(s => s.Id == resourceId)
                .Select(s => (int?)s.Activities.First().StructureId)
                .FirstOrDefaultAsync();

        if (structureId == userStructureId)
            context.Succeed(requirement);
    }
}
