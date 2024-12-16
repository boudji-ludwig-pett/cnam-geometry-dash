# Unity

Documentation about the basics of [Unity](https://unity.com/) (cross-platform game engine) and most useful things to know, to develop a 2D game.

## Links

- [Unity 6 User Manual](https://docs.unity3d.com/Manual/index.html)
- [YouTube (Game Maker's Toolkit) - The Unity Tutorial For Complete Beginners](https://www.youtube.com/watch?v=XtQMytORBmM)

## Unity Interface (4 panels by default)

- Project (bottom): Contains everything that is in our game, like sprites (2D graphical object/image), sound effects, scripts, fonts, etc. Some of them are made in Unity. We can also import files from our computer.
- Hierarchy (left): Contains all the stuff that's in the current scene.
- Scene (middle): A **scene** is a level in our game. Scene are made up of **GameObjects**.
- Inspector (right): Shows the properties and **components** of the selected **GameObject**.

## GameObject

`GameObject` is an invisible container. Properties:

- **Name**
- **Transform**
  - a Position (`X`, `Y`, `Z`)
  - a Rotation (`X`, `Y`, `Z`)
  - a Scale (`X`, `Y`, `Z`)
- **Components**: to add extra features (e.g: `SpriteRenderer` to display a sprite, `BoxCollider2D` to detect collisions, etc).

To create a new `GameObject`, we can right-click in the `Hierarchy` panel and select `Create Empty`.

`GameObject` can have children `GameObject`s. Nesting `GameObject`s is useful to group objects together, and for example, move them all at once, just by moving the parent `GameObject`.

## Components

### SpriteRenderer

The `SpriteRenderer` component renders the Sprite and controls how it visually appears in a Scene. Fields:

- `Sprite`: to select the sprite to render. We can drag a sprite from the `Project` panel.

### Camera

The `Camera` component renders the scene from the point of view of the camera. Fields:

- `Size`: to zoom in and out.
- `Background`: background color.

### Rigidbody2D

The `Rigidbody2D` component allows a `GameObject` to react to physics (gravity, mass).

### Collider2D

The `Collider2D` component allows a `GameObject` to interact with other `GameObject`s. It allows to detect collisions (control hitboxes).

There are different types of colliders:

- `BoxCollider2D`: a box-shaped collider.
- `CircleCollider2D`: a circle-shaped collider.

etc.

### Script

Allows to make our own custom component with C# code.

`MonoBehaviour` is the base class from which script derives. It offers life cycle functions: `Start`, `Update`, `OnEnable` etc.

It has a property called `gameObject` which is the `GameObject` to which the script is attached. For example, to change the name of the `GameObject` to which the script is attached, we can write `gameObject.name = "New Name";`.

```csharp
using UnityEngine;

public class BirdScript : MonoBehaviour
{
  // Start is called once before the first execution of Update after the MonoBehaviour is created
  public void Start()
  {

  }

  // Update is called every frame over, over and over again, run as often as it can depending on the frame rate and computer performance
  public void Update()
  {

  }
}
```

## UnityEngine C#

### Script communication with other components (for example with `Rigidbody2D`)

By default, the script is unaware of the other components attached to the `GameObject`.

To access the `Rigidbody2D` component from the script, we can create a public field of type `Rigidbody2D` in the script.

```csharp
using UnityEngine;

public class BirdScript : MonoBehaviour
{
  public Rigidbody2D myRigidBody;
}
```

In the Unity Editor, the `myRigidBody` field will appear in the `Inspector` panel. We can drag the `Rigidbody2D` component from the `GameObject` to the `myRigidBody` field.

### Input

To get input from the player, we can use the `Input` class.

For example, to detect when the player presses the `Space` key, we can use the `Input.GetKeyDown` method.

In the following example, when the player presses the `Space` key, the bird object will jump.

```csharp
using UnityEngine;

public class BirdScript : MonoBehaviour
{
  public Rigidbody2D rigidBody;

  public void Update()
  {
    if (Input.GetKeyDown(KeyCode.Space))
    {
      rigidBody.linearVelocity = Vector2.up * 10;
    }
  }
}
```

### `Time.deltaTime`

`Time.deltaTime` is the interval in seconds from the last frame to the current one.

It's useful to make the game frame rate independent, as it ensures that the game runs at the same speed on different computers, (code inside `Update` method).

```csharp
using UnityEngine;

public class BirdScript : MonoBehaviour
{
  public Rigidbody2D rigidBody;

  public void Update()
  {
    // Move the bird to the right, 5 units per second
    transform.position = transform.position + (5 * Time.deltaTime * Vector3.right);
  }
}
```

### `Debug.Log`

`Debug.Log` is a method that prints a message to the console, helpful for debugging.

```csharp
using UnityEngine;

public class BirdScript : MonoBehaviour
{
  public void Start()
  {
    Debug.Log("Hello World!");
  }
}
```

### `Destroy`

To destroy a `GameObject`, we can use the `Destroy` method. Useful for performance optimization, for example to destroy a pipe that is no longer visible on the screen.

For example, to destroy a pipe when it reaches a certain position on the left side of the screen (`deadZone`):

```csharp
using UnityEngine;

public class PipeMoveScript : MonoBehaviour
{
  public float moveSpeed = 5;
  public float deadZone = -45;

  public void Update()
  {
    transform.position = transform.position + (Time.deltaTime * moveSpeed * Vector3.left);

    if (transform.position.x < deadZone)
    {
      Destroy(gameObject);
    }
  }
}
```

### Prefabricated `GameObject`

`GameObject` can be prefabricated. Prefabs are reusable `GameObject`s that can be created and modified in the `Project` panel (like a blueprint), for example to spawn objects in the scene dynamically at runtime, with `Instantiate`.

```csharp
using UnityEngine;

public class PipeSpawnScript : MonoBehaviour
{
  public GameObject pipe;

  public void Start()
  {
    Instantiate(pipe, transform.position, transform.rotation);
  }
}
```

### `[ContextMenu]`

The `[ContextMenu]` attribute allows to add a custom function to the context menu of the script in the Unity Editor.

It's useful for debugging, to be able to call a function directly from the Unity Editor.

```csharp
using UnityEngine;

public class LogicManagerScript : MonoBehaviour
{
  [ContextMenu("AddScore")]
  public void AddScore()
  {
    Debug.Log("Score added!");
  }
}
```
