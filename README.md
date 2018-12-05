# IgniteCachingDistributed
Using Apache Ignite as distributed cache infrastructure in .net Core

## Simple configuration
```
services.AddDistributedIgniteCache(option =>
{
    option.Endpoints = new string[]
        {
            "localhost:11211",
            "localhost:47100",
            "localhost:47500",
            "localhost:49112"
        };
    option.PersistenceEnabled = true;
});
```

## Advanced Configuration
for advance configuration, see https://apacheignite-net.readme.io/docs/
```
IgniteConfiguration customeConfiguration = new IgniteConfiguration
  {
      DiscoverySpi = new TcpDiscoverySpi
      {
          IpFinder = new TcpDiscoveryStaticIpFinder
          {
              Endpoints = new string[]
              {
                  "localhost:11211",
                  "localhost:47100",
                  "localhost:47500",
                  "localhost:49112"
              }
          },
          SocketTimeout = TimeSpan.FromSeconds(0.3)
      },
      IncludedEventTypes = EventType.CacheAll,
      DataStorageConfiguration = new DataStorageConfiguration
      {
          DefaultDataRegionConfiguration = new DataRegionConfiguration
          {
              Name = "defaultRegion",
              PersistenceEnabled = true
          },
          DataRegionConfigurations = new[]
          {
                  new DataRegionConfiguration
                  {
                      // Persistence is off by default.
                      Name = "inMemoryRegion"
                  }
              }
      },
      CacheConfiguration = new[]
      {
          new CacheConfiguration
          {
              // Default data region has persistence enabled.
              Name = "persistentCache"
          },
          new CacheConfiguration
          {
              Name = "inMemoryOnlyCache",
              DataRegionName = "inMemoryRegion"
          }
      }
  };
  services.AddDistributedIgniteCache(option => 
  { 
      option.Configuration = customeConfiguration;
      option.SetActive = true;
  });
  ```
