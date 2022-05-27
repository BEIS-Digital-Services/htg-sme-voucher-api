global using System;
global using System.Text;
global using System.Text.Json;
global using System.ComponentModel.DataAnnotations;
global using System.ComponentModel.DataAnnotations.Schema;

global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.Options;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.EntityFrameworkCore;

global using Beis.HelpToGrow.Api.Voucher.Controllers;
global using Beis.HelpToGrow.Api.Voucher.Models;
global using Beis.HelpToGrow.Api.Voucher.Services;
global using Beis.HelpToGrow.Api.Voucher.Interfaces;
global using Beis.HelpToGrow.Api.Voucher.Config;

global using Beis.HelpToGrow.Api.Data.Repositories;

global using Beis.Htg.VendorSme.Database;
global using Beis.Htg.VendorSme.Database.Models;

global using Beis.HelpToGrow.Repositories.Enums;
global using Beis.HelpToGrow.Repositories.Interfaces;
global using Beis.HelpToGrow.Repositories;


global using Beis.HelpToGrow.Common.Enums;
global using Beis.HelpToGrow.Common.Interfaces;
global using Beis.HelpToGrow.Common.Services;
global using Beis.HelpToGrow.Common.Models;




//global using System;
//global using System.Text;
//global using System.Text.Json;
//global using System.ComponentModel.DataAnnotations;
//global using System.ComponentModel.DataAnnotations.Schema;

//global using Microsoft.Extensions.Options;
//global using Microsoft.AspNetCore.Mvc;
//global using Microsoft.EntityFrameworkCore;

//global using Beis.HelpToGrow.Api.Voucher.Enums;
//global using Beis.HelpToGrow.Api.Voucher.Models;
//global using Beis.HelpToGrow.Api.Voucher.Services;
//global using Beis.HelpToGrow.Api.Voucher.Interfaces;
//global using Beis.HelpToGrow.Api.Voucher.Config;

//global using Beis.HelpToGrow.Data.Interfaces;
//global using Beis.HelpToGrow.Data.Repositories;
//global using Beis.HelpToGrow.Voucher.Interfaces;
//global using Beis.HelpToGrow.Api.Data.Repositories;

//global using Beis.Htg.VendorSme.Database;
//global using Beis.Htg.VendorSme.Database.Models;
