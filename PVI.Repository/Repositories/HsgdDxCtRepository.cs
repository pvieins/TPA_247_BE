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
    public class HsgdDxCtRepository : GenericRepository<HsgdDxCt>, IHsgdDxCtRepository
    {
        public HsgdDxCtRepository(GdttContext context, Pvs2024Context context_pias, Serilog.ILogger logger, IConfiguration conf) : base(context, context_pias, logger, conf)
        {


        }
    }
}