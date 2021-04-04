using UnityEngine;
using UnityEngine.UI;

public class Controller : MonoBehaviour {
  public Game game;
  public SpriteRenderer sr;
  public Rigidbody2D rb;
  public Animator anim;
  public Camera cam;
  public float speed = 10;
  public float MaxVelocity;
  public Vector2 jumpForce = new Vector2(0, 10);
  bool isGrounded = false;
  public Text debug;
  public Text debug2;
  public TileType[] room;

  float digAnimTime = 0;
  int toDigX, toDigY;

  // Update is called once per frame
  void Update() {
    Vector2 force = Vector2.zero;
    float x = Input.GetAxis("Horizontal");
    force.x = x * speed;
    rb.AddForce(force);
    if (x < 0) sr.flipX = true;
    else if (x > 0) sr.flipX = false;
    if (x == 0) {
      rb.velocity *= .9f;
    }

    if (digAnimTime > 0) {
      digAnimTime -= Time.deltaTime;
      if (digAnimTime <= 0) {
        game.Dig(toDigX, toDigY);
      }
    }
    else if (rb.velocity.x > .2f || rb.velocity.x < -.2f) {
      anim.SetInteger("Dig", 0);
      anim.SetBool("Walking", true);
    }
    else {
      anim.SetInteger("Dig", 0);
      anim.SetBool("Walking", false);
    }

    if (Input.GetKeyDown(KeyCode.Space) && isGrounded) {
      rb.AddForce(jumpForce);
      isGrounded = false;
    }

    Vector2Int pos = Vector2Int.zero;
    pos.x = (int)transform.position.x - 1;
    pos.y = -(int)transform.position.y;

    Vector2 vel = rb.velocity;
    if (vel.x > MaxVelocity) vel.x = MaxVelocity;
    if (vel.x < -MaxVelocity) vel.x = -MaxVelocity;
    rb.velocity = vel;

    Vector2 mouse = cam.ScreenToWorldPoint(Input.mousePosition);
    Dir dir;
    if (Mathf.Abs(mouse.y - transform.position.y) < .7f) {
      if (mouse.x > transform.position.x + .25f) dir = Dir.Right;
      else if (mouse.x < transform.position.x - .25f) dir = Dir.Left;
      else dir = Dir.None;
    }
    else if (Mathf.Abs(mouse.x - transform.position.x) < .7f) {
      if (mouse.y < transform.position.y - .25f) dir = Dir.Down;
      else if (mouse.y > transform.position.y + .25f) dir = Dir.Up;
      else dir = Dir.None;
    }
    else dir = Dir.None;

    int px = (int)(transform.position.x + .5f);
    int py = -Mathf.RoundToInt(transform.position.y - 0f);
    debug.text = px + "," + py;

    if (dir != Dir.None) {

      if (dir == Dir.Right) px++;
      if (dir == Dir.Left) px--;
      if (dir == Dir.Up) py--;
      if (dir == Dir.Down) py++;

      if (px < 0 || px >= game.gridWidth || py < 0 || py >= game.gridHeight)
        debug2.text = px + ", " + py + " : " + dir.ToString();
      else {
        TileType tile = game.room[px + game.gridWidth * py];
        if (tile != TileType.Empty && tile != TileType.Wall && Input.GetMouseButtonDown(0)) {
          anim.SetInteger("Dig", 2);
          toDigX = px; // To remember where to dig
          toDigY = py;
          digAnimTime = .8f;
          sr.flipX = dir == Dir.Left;
        }
        debug2.text = px + ", " + py + " : " + dir + " / " + game.room[px + game.gridWidth * py];
      }
    }
    else {
      debug2.text = "";
    }
    debug.text += " -> " + px + "," + py;

  }
  void OnCollisionEnter2D(Collision2D collision) {
    isGrounded = true;
  }

  public enum Dir { None, Up, Down, Left, Right }
}

