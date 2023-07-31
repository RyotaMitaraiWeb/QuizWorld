# JwtBlacklist

## Remarks
This service uses a Redis database to maintain a list of blacklisted JWTs. The blacklisted JWTs cannot be used in authorized requests, even if they are otherwise valid. Also note that this service, unlike others, is registered as a singleton; this is because it has a dependency that is also registered as a singleton.

## Methods
```cs
public JwtBlacklistService(RedisConnectionProvider redisProvider)
```

### Usage
Creates a new instance of the class, configuring dependencies based on the arguments.


```cs
public Task<string?> FindJWT(string jwt)
```

### Usage
Attempts to find an item whose ID equals the provided ``jwt``.

### Returns
- A Task that resolves to the JWT found, if the Redis collection contains an item with the JWT as a key
- A Task that resolves to ``null`` otherwise.


```cs
public Task<bool> BlacklistJWT(string jwt)
```

### Usage
Attempts to insert the provided ``jwt`` in the JWT blacklist. If the token is inserted successfully, it will be automatically removed after 24 hours (the standard timespan of a JWT for this project).

### Returns
A Task that resolves to a boolean value that indicates whether the token was blacklisted successfully.