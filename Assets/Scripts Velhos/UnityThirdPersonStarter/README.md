# Unity Third Person (CharacterController + Input System)

Conteúdo:
- `Scripts/PlayerMotor.cs` — movimento com CC, corrida, pulo, gravidade, animação opcional e interação.
- `Scripts/CameraOrbit.cs` — câmera orbital 3ª pessoa com look e zoom.
- `Input/PlayerControls.inputactions` — mapa de ações pronto.

## Como usar
1. Copie a pasta para `Assets/` do seu projeto **ou** importe o .zip no Explorer e arraste para o Unity.
2. Certifique-se que o projeto usa **Input System**: `Edit → Project Settings → Player → Active Input Handling = Input System Package`.
3. Na cena:
   - Crie um objeto `Player` com `CharacterController` e `PlayerMotor`.
   - Crie um filho do Player chamado `Pivot` (altura ~1.5m).
   - Crie `CameraRig` na raiz e adicione `CameraOrbit`. Crie um filho `CameraHolder` e ponha a `Main Camera` dentro.
   - Arraste as referências: `PlayerMotor.cameraTransform = Main Camera`, `groundCheck = vazio no pé`.
   - Em `CameraOrbit`, arraste: `followTarget = Player`, `pivot = Pivot`, `cameraHolder = CameraHolder`.
4. Abra `Input/PlayerControls.inputactions` e `Save Asset` (o Unity gera o C#).
5. Arraste as Actions para os campos dos scripts (Move/Look/Jump/Sprint/Interact/Zoom).

## Dicas
- Ajuste `groundMask` e `groundCheckRadius` para seu terreno.
- Para animação, configure parâmetros: `Speed` (float), `Grounded` (bool), `Jump` (trigger), `Interact` (trigger).
- Velocidades e sensibilidade são editáveis no inspetor.

Compatível com Unity 2021.3+ com Input System 1.5+.
