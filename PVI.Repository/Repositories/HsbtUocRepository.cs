using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PVI.DAO.Entities.Models;
using PVI.Repository.Interfaces;
using System.Collections;
using System.Collections.Generic;

namespace PVI.Repository.Repositories
{
    public class HsbtUocRepository : GenericRepository<HsbtUoc>, IHsbtUocRepository
    {
        public HsbtUocRepository(GdttContext context, Pvs2024Context context_pias, Pvs2024UpdateContext context_pias_update, Serilog.ILogger logger, IConfiguration conf) : base(context, context_pias,context_pias_update, logger, conf)
        {


        }
    }
}