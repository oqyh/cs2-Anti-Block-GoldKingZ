## .:[ Join Our Discord For Support ]:.
<a href="https://discord.com/invite/U7AuQhu"><img src="https://discord.com/api/guilds/651838917687115806/widget.png?style=banner2"></a>

***
# [CS2] Anti-BlockNade-GoldKingZ (1.0.1)

### Anti-Block Body/Nades

![antiblocknade](https://github.com/user-attachments/assets/bd580a5b-a833-4a49-9256-7740cdb7d4fb)

![antibodyblock](https://github.com/user-attachments/assets/85217774-b475-4b9f-a2b6-465dfc0abbeb)


## .:[ Dependencies ]:.
[Metamod:Source (2.x)](https://www.sourcemm.net/downloads.php/?branch=master)

[CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp/releases)

[Newtonsoft.Json](https://www.nuget.org/packages/Newtonsoft.Json)

## .:[ Configuration ]:.

> [!CAUTION]
> Config Located In ..\addons\counterstrikesharp\plugins\Anti-Block-GoldKingZ\config\config.json                                           
>

```json
{

//----------------------------[ ↓ BodyBlock Configs ↓ ]-------------------------------

  //OnStartRound Time In Secs Anti Body Block
  //(0) = Disabled
  "AntiBodyBlock_OnStartRoundDurationXInSecs": true,

//----------------------------[ ↓ NadeBlock Configs ↓ ]-------------------------------

  //Enable Anti Block Nade On TeamMates?
  "AntiBlockNades_IfThrowToTeamMates": true,

  //Enable Anti Block Nade On Enemy Team?
  "AntiBlockNades_IfThrowToEnemyTeam": false,

  //Which Nade Do You Want Anti Block
  //hegrenade
  //flashbang
  //smokegrenade
  //decoy
  //molotov
  "AntiBlockNades_TheseNades": "hegrenade,flashbang,smokegrenade,decoy,molotov",

//----------------------------[ ↓ Utilities ↓ ]----------------------------------------------
	
  //Enable Debug Will Print Server Console If You Face Any Issue
  "EnableDebug": false,
}
```

![329846165-ba02c700-8e0b-4ebe-bc28-103b796c0b2e](https://github.com/oqyh/cs2-Game-Manager/assets/48490385/3df7caa9-34a7-47da-94aa-8d682f59e85d)


## .:[ Language ]:.
```json
{
	//==========================
	//        Colors
	//==========================
	//{Yellow} {Gold} {Silver} {Blue} {DarkBlue} {BlueGrey} {Magenta} {LightRed}
	//{LightBlue} {Olive} {Lime} {Red} {Purple} {Grey}
	//{Default} {White} {Darkred} {Green} {LightYellow}
	//==========================
	//        Other
	//==========================
	//{0} = Time Anti-BodyBlock
	//{nextline} = Print On Next Line
	//==========================
	
    "PrintChatToAllPlayers.AntiBlock.Enabled": "{green}Gold KingZ {grey}| {grey}Anti-BodyBlock Is Now {lime}Enabled {grey}For {lime}{0} Secs",
    "PrintChatToAllPlayers.AntiBlock.Disabled": "{green}Gold KingZ {grey}| {grey}Anti-BodyBlock Is Now {darkred}Disabled"
}
```

## .:[ Change Log ]:.
```
(1.0.1)
-Added AntiBodyBlock_OnStartRoundDurationXInSecs

(1.0.0)
-Initial Release
```

## .:[ Donation ]:.

If this project help you reduce time to develop, you can give me a cup of coffee :)

[![paypal](https://www.paypalobjects.com/en_US/i/btn/btn_donateCC_LG.gif)](https://paypal.me/oQYh)
