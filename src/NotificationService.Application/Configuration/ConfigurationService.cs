using System;
using System.Collections.Generic;
using System.Text;

namespace NotificationService.Application.Configuration;

public class ConfigurationService {
    private Configuration _configuration;
    public ConfigurationService(Configuration initialConfiguration) {
        _configuration = initialConfiguration;
        ApplyConfiguration(_configuration);
    }

    public Configuration Configuration => _configuration;

    public void ApplyConfiguration(Configuration newConfiguration) {
        throw new NotImplementedException();
    }
}
