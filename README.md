# UserManager
scaffolded project for a web application that use authentication, authorization and users managemnt
---

# Current bug
in [JwtProvider](https://github.com/Friggi02/UserManager/blob/master/backend/Project.Dal/Jwt/JwtProvider.cs) vado a generare il bearer token di autenticazione inserendo nei claim tutti i permessi.
Questo token, analizzato con [Jwt.io](https://jwt.io/), appare come segue:
``` json
{
  "sub": "e5521f4c-c677-4b6e-81e4-e0dcd8a0ea2d",
  "permissions": [
    "ManageMyself",
    "ManageUsers"
  ],
  "exp": 1701381334,
  "iss": "http://localhost:5000",
  "aud": "http://localhost:4200"
}
```
in [UsersController](https://github.com/Friggi02/UserManager/blob/master/backend/Project.WebApi/Controllers/UsersController.cs) vorrei estrarre lo userId dal token.
negli scorsi progetti mi bastava fare
``` cs
User.FindFirstValue(ClaimTypes.NameIdentifier)
```
per ottenere l'id dello user che faceva la richiesta ma questa volta non funzionava. ho risolto facendo:
``` cs
public string GetIdFromRequest(HttpRequest request)
{
    string tokenEncoded = request.Headers[HeaderNames.Authorization].SingleOrDefault()!.Replace("Bearer ", "");
    JwtSecurityToken tokenDecoded = new JwtSecurityTokenHandler().ReadJwtToken(tokenEncoded);

    return tokenDecoded.Claims.First(c => c.Type == "sub").Value;
}
```
ho un problema molto simile nel [PermissionAuthorizationHandler](https://github.com/Friggi02/UserManager/blob/master/backend/Project.Dal/Permit/PermissionAuthorizationHandler.cs) nel quale non c'è verso di fargli leggere i claim dei permessi nonostate nel payload del token siano evidenti.

penso che i due problemi abbiano una radice comune che non riesco ad individuare.
