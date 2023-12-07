# Logging

## Logging

Configure logging in `NLog.config` and copy this file to publish directory. 

Rules to filter out all unnecessary log messages.

```xml
<nlog>
  <rules>
    <logger name="System.*" finalMinLevel="Warn" />
    <logger name="Microsoft.*" finalMinLevel="Warn" />
    <logger name="Microsoft.Hosting.Lifetime*" finalMinLevel="Info" />
    <logger name="*" minlevel="Info" writeTo="console, file" />
  </rules>
</nlog>
```
