# CacheProvider #
=============



## Mongo Cache Provider ##

> Uses a Mongo Database for cache storage

Example of a parm colleciton

    private readonly NameValueCollection _disabledSettings = new NameValueCollection
            {
                {"application", "Cache"},
                {"timeout", " 5"},
                {"host", "127.0.0.1"},
                {"port","27017"},
                {"env", "UnitTest"},
                {"enable", "true"}
            };


## Memory Cache Provider ##

> Uses the .Net provided MemoryCache

Example of a parm colleciton

    private readonly NameValueCollection _disabledSettings = new NameValueCollection
            {
                {"application", "Cache"},
                {"timeout", " 5"},
                {"enable", "true"}
            };



----------

Example Unity Config Settings

    <unity>
    <assembly name="CacheProvicer" />
    <assembly name="CacheProvicer.Memory" />

    <container name="Main">

      <register type="ICacheProvider" mapTo="MemoryCacheProvider">
        <lifetime type="ContainerControlledLifetimeManager" />
        <constructor>
         
        </constructor>
        <method name="Initialize">
          <param name="name" value="ICacheProvider" />
          <param name="config">
            <value value="timeout=1;host=none;port=000;env=non;enable=true" typeConverter="NameValueCollectionTypeConverter" />
          </param>
        </method>
      </register>
    </container> </unity>