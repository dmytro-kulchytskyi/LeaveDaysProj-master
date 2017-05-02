using leavedays.Models;
using leavedays.Models.Repository.Interfaces;
using leavedays.Models.ViewModel;
using leavedays.Models.ViewModels.License;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace leavedays.Services
{
    public class LicenseService
    {
        private readonly IUserRepository userRepository;
        private readonly ILicenseRepository licenseRepository;
        private readonly ICompanyRepository companyRepository;
        private readonly IInvoiceRepository invoiceRepository;
        private readonly IModuleRepository moduleRepository;
        private readonly IDefaultModuleRepository defaultModuleRepository;
        private readonly IDefaultLicenseRepository defaultLicenseRepository;
        private readonly IModuleChangeRepository moduleChangeRepository;

        public LicenseService(
            IModuleChangeRepository moduleChangeRepository,
          IUserRepository userRepository,
          ILicenseRepository licenseRepository,
          ICompanyRepository companyRepository,
          IInvoiceRepository invoiceRepository,
          IModuleRepository moduleRepository,
          IDefaultModuleRepository defaultModuleRepository,
          IDefaultLicenseRepository defaultLicenseRepository)
        {
            this.moduleChangeRepository = moduleChangeRepository;
            this.userRepository = userRepository;
            this.licenseRepository = licenseRepository;
            this.companyRepository = companyRepository;
            this.invoiceRepository = invoiceRepository;
            this.moduleRepository = moduleRepository;
            this.defaultLicenseRepository = defaultLicenseRepository;
            this.defaultModuleRepository = defaultModuleRepository;
        }


        public Result<License> CreateLicense(string name)
        {
            var defaultLicense = defaultLicenseRepository.GetByName(name);
            if (defaultLicense == null)
            {
                return null;
            }
            return CreateLicense(defaultLicense);
        }

        public Result<License> CreateLicense(int id)
        {
            var defaultLicense = defaultLicenseRepository.GetById(id);
            if (defaultLicense == null)
            {
                return null;
            }
            return CreateLicense(defaultLicense);
        }

        public Result<License> CreateLicense(DefaultLicense defaultLicense)
        {
            if (defaultLicense == null)
            {
                return Result<License>.Error();
            }

            var license = new License()
            {
                DefaultLicenseId = defaultLicense.Id,
                Price = defaultLicense.Price,
                LicenseCode = Guid.NewGuid().ToString(),
                Seats = 1
            };

            var licenseId = licenseRepository.Save(license);
            if (licenseId == 0)
            {
                licenseRepository.Delete(license);
                return Result<License>.Error();
            }

            var modules = defaultLicense.DefaultModules.Select(defaultModule => new Module()
            {
                DefaultModuleId = defaultModule.Id,
                Price = defaultModule.Price,
                IsActive = true,
                LicenseId = license.Id
            });
            var ids = moduleRepository.Save(modules);

            return Result<License>.Success(license);
        }

        public List<LicenseInfo> GetLicenseInfoList()
        {
            var result = licenseRepository.GetLicenseInformation().ToList();
            return result;
        }

        public List<LicenseInfo> GetSearchedLicenseInfo(string searchLine)
        {
            var result = licenseRepository.GetSearchedInformation(searchLine).ToList();
            return result;
        }

        public List<LicenseInfo> GetAdwenchedSearchLicenseInfo(SearchOption option)
        {
            var result = licenseRepository.GetAdwenchedSearchedInformation(option).ToList();
            return result;
        }

        public IList<ModuleShortInfo> GetModulesShortInfo(License license, bool? moduleStatus = null)
        {
            return GetDefaultModules(license, moduleStatus).Select(m => new ModuleShortInfo()
            {
                DefaultModuleId = m.Id,
                Name = m.Name,
                Price = m.Price
            }).ToList();
        }

        public IList<DefaultModule> GetDefaultModules(License license, bool? moduleStatus = null)
        {

            var modules = moduleRepository.GetByLicenseId(license.Id, moduleStatus);
                modules = modules.Where(m => !m.IsLocked).ToList();

            var defaultModules = modules.Select(module =>
            {
                var m = defaultModuleRepository.GetById(module.DefaultModuleId);
                m.Price = module.Price;
                return m;
            }).ToList();

            return defaultModules;

        }
        public IEnumerable<ModuleInfo> GetModulesInfo()
        {
            var licenses = defaultLicenseRepository.GetAll();
            var modules = defaultModuleRepository.GetAll();
            var modulesInfo = modules.Select(m => new ModuleInfo()
            {
                Id = m.Id,
                Name = m.Name,
                Price = m.Price,
                Description = m.Description,
                Licenses = licenses.Where(l => l.DefaultModules.Select(k => k.Id).Contains(m.Id))
            });
            return modulesInfo;
        }

        public ModuleInfo GetModuleInfo(int id)
        {
            var module = defaultModuleRepository.GetById(id);
            var licenses = defaultLicenseRepository.GetByModuleId(module.Id);
            ModuleInfo moduleInfo = new ModuleInfo()
            {
                Id = module.Id,
                Name = module.Name,
                Description = module.Description,
                Price = module.Price,
                Licenses = licenses
            };
            return moduleInfo;
        }

        public int CreateModule(EditModuleModel editModule, string[] selectedLicenses)
        {
            DefaultModule defaultModule = new DefaultModule()
            {
                Name = editModule.Name,
                Description = editModule.Description,
                Price = editModule.Price
            };
            defaultModuleRepository.Save(defaultModule);
            if (selectedLicenses != null)
            {
                var defaultLicenses = defaultLicenseRepository.GetByNames(selectedLicenses.ToList());
                foreach (var license in defaultLicenses)
                {
                    license.DefaultModules.Add(defaultModule);
                }
                defaultLicenseRepository.Save(defaultLicenses.ToList());
                var licenses = licenseRepository.GetByDefaultLicenseIds(defaultLicenses.Select(m => m.Id).ToArray());
                foreach (var license in licenses)
                {
                    Module module = new Module()
                    {
                        DefaultModuleId = defaultModule.Id,
                        Price = defaultModule.Price,
                        LicenseId = license.Id,
                        IsActive = false,
                    };
                    moduleRepository.Save(module);
                }
            }
            return defaultModule.Id;
        }



        public void EditModule(EditModuleModel editModule, string[] selectedLicenses)
        {
            var module = defaultModuleRepository.GetById(editModule.Id);
            module.Name = editModule.Name;
            module.Description = editModule.Description;
            module.Price = editModule.Price;
            var selectLicenses = defaultLicenseRepository.GetByNames(selectedLicenses.ToList());
            var allLicenses = defaultLicenseRepository.GetAll();
            if (selectedLicenses != null)
            {
                foreach (var license in allLicenses)
                {
                    if (selectLicenses.Select(m => m.Id).Contains(license.Id) && !license.DefaultModules.Select(l => l.Id).Contains(module.Id))
                    {
                        license.DefaultModules.Add(module);
                    }
                    else if (!selectLicenses.Select(m => m.Id).Contains(license.Id) && license.DefaultModules.Select(l => l.Id).Contains(module.Id))
                    {
                        license.DefaultModules = new HashSet<DefaultModule>(license.DefaultModules.Where(m => m.Id != module.Id));
                    }
                }
            }
            else
            {
                foreach (var license in allLicenses)
                {
                    license.DefaultModules = new HashSet<DefaultModule>(license.DefaultModules.Where(m => m.Id != module.Id));
                }
            }
            defaultLicenseRepository.Save(allLicenses.ToList());
            defaultModuleRepository.Save(module);
        }

        public Result<License> EditModules(int userId, IEnumerable<ModuleShortInfo> moduleStatusList, bool setEnable)
        {
            var user = userRepository.GetById(userId);
            if (user == null)
            {
                return Result<License>.Error();
            }

            var companyId = user.CompanyId;

            var company = companyRepository.GetById(companyId);
            if (company == null)
            {
                return Result<License>.Error();
            }

            var licenseId = company.LicenseId;
            var switchedModulesIds = moduleStatusList.Where(m => m.Checked).Select(m => m.DefaultModuleId).ToList();
            var allModules = moduleRepository.GetByLicenseId(licenseId);

            if (allModules == null || allModules.Count == 0)
            {
                return Result<License>.Error();
            }

            var switchedModules = allModules.Where(m => switchedModulesIds.Contains(m.DefaultModuleId)).ToList();

            if (switchedModules != null && switchedModules.Count != 0)
            {
                foreach (var module in switchedModules)
                {
                    if (module != null && module.LicenseId == licenseId)
                    {
                        if (module.IsActive != setEnable)
                        {
                            if (!module.IsLocked)
                            {
                                module.IsActive = setEnable;
                                moduleRepository.Save(module);
                            }
                        }
                    }
                }
            }

            return Result<License>.Success(licenseRepository.GetById(licenseId));
        }

        public string EditCustomerModules(int licenseId, ModuleInfo[] modules, string startDate = "")
        {
            var defaultModules = moduleRepository.GetByLicenseId(licenseId);
            List<ModuleChange> modulesChange = new List<ModuleChange>();
            List<ModuleInfo> modulesInfo = new List<Models.ViewModels.License.ModuleInfo>();
            foreach (var defModule in defaultModules)
            {
                var res = modules.Where(m => m.Id == defModule.Id && (m.isLocked != defModule.IsLocked || m.Price != defModule.Price));
                if (res.Count() > 0)
                {
                    modulesInfo.Add(res.First());
                }
            }
            if (modulesInfo.Count > 0)
            {
                string[] date = startDate.Split('.');
                foreach (ModuleInfo module in modulesInfo)
                {
                    var changeModule = moduleChangeRepository.GetByDate(int.Parse(date[1]), int.Parse(date[0]), module.Id);
                    if(changeModule.Count != 0)
                    {
                        moduleChangeRepository.Delete(changeModule.First());
                    }
                    ModuleChange moduleChange = new ModuleChange()
                    {
                        ModuleId = module.Id,
                        IsLocked = module.isLocked,
                        Price = module.Price,
                        StartDate = new DateTime(int.Parse(date[1]), int.Parse(date[0]), 1)
                    };
                    modulesChange.Add(moduleChange);
                }
                moduleChangeRepository.Save(modulesChange);
                return "All changes was saved";
            }
            return "It is nothing to change";
        }

        public License GetLicenseByUserId(int userId)
        {
            var user = userRepository.GetById(userId);

            var company = companyRepository.GetById(user.CompanyId);
            var license = licenseRepository.GetById(company.LicenseId);

            return license;
        }

        public LicenseInformation GetLicenseInfo(int userId)
        {
            var user = userRepository.GetById(userId);

            var company = companyRepository.GetById(user.CompanyId);
            var license = licenseRepository.GetById(company.LicenseId);

            var defaultLicense = defaultLicenseRepository.GetById(license.DefaultLicenseId);

            var licenseInfo = new LicenseInformation()
            {
                Company = company,
                License = license,
                LicenseName = defaultLicense.Name,
                LicenseCode = license.LicenseCode,
                LicensesCount = license.Seats,
                ActiveLicensesCount = companyRepository.GetUsersCount(company.Id),
                Price = license.Price
            };
            return licenseInfo;
        }

        public int EditLicenseSeats(int userId, int count)
        {
            var user = userRepository.GetById(userId);
            if (user == null) return 0;

            var company = companyRepository.GetById(user.CompanyId);
            if (company == null) return 0;

            var activeSeats = companyRepository.GetUsersCount(company.Id);

            var license = licenseRepository.GetById(company.Id);
            if (license == null) return 0;

            if (license.Seats - activeSeats + count < 0)
                return 0;

            license.Seats += count;
            licenseRepository.Save(license);

            return 1;
        }
    }
}