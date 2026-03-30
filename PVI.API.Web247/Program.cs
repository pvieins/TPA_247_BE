
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using PVI.DAO.Entities.Models;
using PVI.Repository.Repositories;
using PVI.Service;
using PVI.Service.Request;
using Serilog;
using PVI.Helper;
using PVI.Repository.Interfaces;
using PVI.Service.ActionProcess;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Office.Interop.Word;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
Log.Logger = new LoggerConfiguration()
             .WriteTo.File(builder.Configuration["Jwt:log_path"]!, rollingInterval: RollingInterval.Day)
             .CreateLogger();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{


    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "PVI API GDTT",
        Version = "v1 " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
        Description = "Thời gian " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
    });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"Xác thực JWT sử dụng Bearer scheme.  
                      Nhập 'Bearer [Token]' vào ô dưới.
                      Ví dụ: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                  new OpenApiSecurityScheme
                  {
                    Reference = new OpenApiReference
                      {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                      },
                      Scheme = "oauth2",
                      Name = "Bearer",
                      In = ParameterLocation.Header,
                    },
                    new List<string>()
                  }
             });
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;

    //TUNGDV1
    //Bat cau hinh check token khi goi API
    //Gui lai Secret key khi tao token
    //options.TokenValidationParameters = new TokenValidationParameters
    //{
    //    ValidateIssuer = true,
    //    ValidateAudience = true,
    //    ValidateLifetime = true,
    //    ValidateIssuerSigningKey = true,
    //    ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
    //    ValidAudience = builder.Configuration["JWT:ValidAudience"],
    //    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
    //};
    
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        //Cau hinh he thong test
        //ValidateLifetime = true,
        //ValidateIssuer = false,
        //ValidateAudience = false,

        //Cau hinh he thong live
        //TUNGDV1
        ValidateLifetime = true,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidAudience = builder.Configuration["Jwt:ValidAudience"],
        ValidIssuer = builder.Configuration["Jwt:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]))
    };
});
builder.Services.AddHttpClient();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()   // Allow any frontend (public access)
                  .AllowAnyMethod()   // Allow any HTTP method (GET, POST, PUT, DELETE, etc.)
                  .AllowAnyHeader()  // Allow any headers

            .WithExposedHeaders("X-Pagination");
        });
});
//builder.Services.AddCors(options => options.AddDefaultPolicy(policy => policy
//    .AllowAnyHeader()
//    .AllowAnyMethod()
//    .AllowAnyOrigin()

//    .WithExposedHeaders("X-Pagination")
//));
builder.Services.AddDbContext<GdttContext>(option =>
{
    option.ConfigureWarnings(w => w.Ignore(SqlServerEventId.DecimalTypeKeyWarning)).UseSqlServer(builder.Configuration.GetConnectionString("GdttContext")!);

});
builder.Services.AddDbContext<Pvs2024Context>(option =>
{
    option.ConfigureWarnings(w => w.Ignore(SqlServerEventId.DecimalTypeKeyWarning)).UseSqlServer(builder.Configuration.GetConnectionString("PiasContext")!);

});
builder.Services.AddDbContext<Pvs2024UpdateContext>(option =>
{
    option.ConfigureWarnings(w => w.Ignore(SqlServerEventId.DecimalTypeKeyWarning)).UseSqlServer(builder.Configuration.GetConnectionString("PiasUpdateContext")!);

});
builder.Services.AddDbContext<MY_PVIContext>(option =>
{
    option.ConfigureWarnings(w => w.Ignore(SqlServerEventId.DecimalTypeKeyWarning)).UseSqlServer(builder.Configuration.GetConnectionString("MyPVIContext")!);

});
builder.Services.AddDbContext<Pvs2024TToanContext>(option =>
{
    option.ConfigureWarnings(w => w.Ignore(SqlServerEventId.DecimalTypeKeyWarning)).UseSqlServer(builder.Configuration.GetConnectionString("PiasTToanContext")!);

});

builder.Services.AddDbContext<PvsHDDTContext>(option =>
{
    option.ConfigureWarnings(w => w.Ignore(SqlServerEventId.DecimalTypeKeyWarning)).UseSqlServer(builder.Configuration.GetConnectionString("PiasHDDTContext")!);

});
builder.Services.AddDbContext<PvsTcdContext>(option =>
{
    option.ConfigureWarnings(w => w.Ignore(SqlServerEventId.DecimalTypeKeyWarning)).UseSqlServer(builder.Configuration.GetConnectionString("PvsTcdContext")!);

});

builder.Services.Configure<DownloadSettings>(builder.Configuration.GetSection("DownloadSettings"));
builder.Services.Configure<Word2PdfSettings>(builder.Configuration.GetSection("Word2PdfSettings"));
builder.Services.Configure<UploadSettings>(builder.Configuration.GetSection("UploadSettings"));
builder.Services.AddSingleton(Log.Logger);
builder.Services.AddScoped<IHsgdCtuRepository, HsgdCtuRepository>();
builder.Services.AddScoped<HsgdCtuService>();
builder.Services.AddScoped<IHsgdTtrinhRepository, HsgdTtrinhRepository>();
builder.Services.AddScoped<HsgdTtrinhService>();
builder.Services.AddScoped<IHsgdDnttRepository, HsgdDnttRepository>();
builder.Services.AddScoped<HsgdDnttService>();
builder.Services.AddScoped<IHsgdTtrinhCtRepository, HsgdTtrinhCtRepository>();
builder.Services.AddScoped<IHsgdTotrinhXmlRepository, HsgdTotrinhXmlRepository>();
builder.Services.AddScoped<DmGaraService>();
builder.Services.AddScoped<IDmGaraRepository, DmGaraRepository>();

builder.Services.AddScoped<DiemTrucService>();
builder.Services.AddScoped<IDiemtrucRepository, DiemTrucRepository>();

builder.Services.AddScoped<PQuyenKyHsService>();
builder.Services.AddScoped<IPquyenKyHsRepository, PquyenKyHsRepository>();

builder.Services.AddScoped<DmUserService>();
builder.Services.AddScoped<IDmUserRepository, DmUserRepository>();

builder.Services.AddScoped<HsgdDxService>();
builder.Services.AddScoped<IHsgdDxRepository, HsgdDxRepository>();
builder.Services.AddScoped<IHsgdDxCtRepository, HsgdDxCtRepository>();
builder.Services.AddScoped<IHsbtCtRepository, HsbtCtRepository>();
builder.Services.AddScoped<IHsbtUocRepository, HsbtUocRepository>();
builder.Services.AddScoped<IHsbtGdRepository, HsbtGdRepository>();
builder.Services.AddScoped<IHsbtUocGdRepository, HsbtUocGdRepository>();
builder.Services.AddScoped<IHsbtThtsRepository, HsbtThtsRepository>();
builder.Services.AddScoped<LichtrucgdvService>();
builder.Services.AddScoped<ILichTrucGDVRepository, LichtrucgdvRepository>();

builder.Services.AddScoped<DmHmucSuaChuaService>();
builder.Services.AddScoped<IDmHmucSuaChuaRepository, DmHmucSuaChuaRepository>();

builder.Services.AddScoped<DmUyQuyenService>();
builder.Services.AddScoped<IDmUyQuyenRepository, DmUyQuyenRepository>();

builder.Services.AddScoped<DmKhuVucService>();
builder.Services.AddScoped<IDmKhuVucRepository, DmKhuVucRepository>();

builder.Services.AddScoped<DmDeviceService>();
builder.Services.AddScoped<IDmDeviceRepository, DmDeviceRepository>();

builder.Services.AddScoped<DmHieuXeService>();
builder.Services.AddScoped<IDmHieuXeRepository, DmHieuXeRepository>();

builder.Services.AddScoped<BaoCaoService>();
builder.Services.AddScoped<IBaoCaoRepository, BaoCaoRepository>();

builder.Services.AddScoped<DmGaraKhuVucService>();
builder.Services.AddScoped<IDmGaraKhuVucRepository, DmGaraKhuvucRepository>();

builder.Services.AddScoped<KbttCtuService>();
builder.Services.AddScoped<IKbttCtuRepository, KbttCtuRepository>();
builder.Services.AddScoped<IFileAttachBtRepository, FileAttachBtRepository>();

builder.Services.AddAllElasticApm();

//builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
//builder.Services.AddSwaggerGen();
builder.Services.AddAuthorization(options =>
{
    // By default, all incoming requests will be authorized according to the default policy.
    //options.FallbackPolicy = options.DefaultPolicy;
});

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.KeepAliveTimeout = TimeSpan.FromHours(Double.Parse(builder.Configuration["Timeout:apiTimeoutHours"]));
    options.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(Double.Parse(builder.Configuration["Timeout:apiTimeoutMinutes"]));
    options.Limits.MaxRequestBodySize = 20 * 1024 * 1024; // 20MB
    //options.Limits.KeepAliveTimeout = TimeSpan.FromHours(1);
    //options.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(10);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseSwagger();
//app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors("AllowAll");
app.UseAuthentication();   // thông tin đăng nhập (xác thực)
app.UseAuthorization();   // thông tinn về quyền của User

app.MapControllers();

app.Run();
