# Heal Projectile

A TShock plugin to heal player with projectile.

Config should be located in `tshock` directory and it's name must be `HealProjectileConfig.json`. Same directory as config.json.

If there is no config file in directory, this plugin will make config file with below text:

```json
[
  {
    "ProjectileType": 10,
    "HealAmount": 30
  }
]
```

This config will be able to recover 30 hp when Purification Powder hits player.

You can specify multiple projectiles with below form:

```
[
  {
    "ProjectileType": 10,
    "HealAmount": 30
  },
  {
    "ProjectileType": 14,
    "HealAmount": 5
  }
]
```