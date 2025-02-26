using System.Linq;
using src.Dev;

namespace src;

public static class PlayerController
{
    public const f32 TIME_SCALE = 0.85f;
    public const f32 GRAVITY = -10f;
    public const f32 STOMP_SPEED = -40f;
    public const f32 JUMP_STRENGTH = 3.25f;
    public const f32 H_ACC = 40f;
    public const f32 H_DAMP = 60f;
    public const f32 MAX_H_SPEED = 2.25f;
    public const f32 JUMP_VALIDATION_WINDOW = 0.225f;
    public const f32 COYOTE_TIME = 0.175f;

    private static V2f vel;
    private static f32 timeSinceGrounded;
    private static f32 timeSinceJumpAttempt;


    public static GameObject playerObj { get; set; }


    public static void OnLevelLoaded()
    {
        vel = V2f.zero;
        timeSinceGrounded = 0f;
        timeSinceJumpAttempt = 0f;
        playerObj = LevelManager.objs[0];
    }

    public static void Tick(f32 dt)
    {
        if(!LevelManager.ready || playerObj is null)
            return;

        f32 unscaledDt = dt;
        dt = f32.Min(dt * TIME_SCALE, 0.01f);

        vel.y += GRAVITY * dt;

        bool rInput = Input.KeyHeld(Key.Right, Key.D);
        bool lInput = Input.KeyHeld(Key.Left, Key.A);

        if(rInput)
            vel.x += H_ACC * dt;
        if(lInput)
            vel.x -= H_ACC * dt;

        if(!rInput && !lInput)
        {
            vel.x -= f32.Sign(vel.x) * H_DAMP * dt;

            if(f32.Abs(vel.x) < 0.3f)
                vel.x = 0f;
        }

        vel.x = f32.Clamp(vel.x, -MAX_H_SPEED, MAX_H_SPEED);

        bool stomping = Input.KeyHeld(Key.Down, Key.S);
        if(stomping)
            vel.y += STOMP_SPEED * dt;

        if(Input.KeyDown(Key.W, Key.Up, Key.Space))
            timeSinceJumpAttempt = JUMP_VALIDATION_WINDOW;

        if(timeSinceGrounded > 0f && timeSinceJumpAttempt > 0f)
        {
            vel.y = JUMP_STRENGTH;
            timeSinceGrounded = 0f;
            timeSinceJumpAttempt = 0f;
        }

        if(vel.y > 0f && !Input.KeyHeld(Key.W, Key.Up, Key.Space))
            vel.y = 0f;

        V2f newPos = playerObj.loc + vel * dt;

        HandleCollision(ref newPos, ref vel, out bool grounded, out GameObject col);

        if(grounded)
            timeSinceGrounded = COYOTE_TIME;

        timeSinceJumpAttempt -= unscaledDt;
        timeSinceGrounded -= unscaledDt;

        playerObj.loc = newPos;

        //if(col is not null)
        //    switch(col.type)
        //    {
        //        case ObjectType.Danger:
        //        {
        //            GameState.KillPlayer();
        //            break;
        //        }
        //        case ObjectType.Goal:
        //        {
        //            GameState.WinLevel();
        //            break;
        //        }
        //        case ObjectType.Breakable:
        //        {
        //            if(stomping)
        //                col.enabled = false;
        //            else
        //                GameState.KillPlayer();

        //            break;
        //        }
        //        default:
        //            break;
        //    }

        if(!WindowEngine.windows.Any(w => RectsIntersect(playerObj.output.GetLoc(), playerObj.output.GetSize(), w.screenLoc, w.screenSize)))
            Log("Player died :(");
    }


    private static void HandleCollision(ref V2f newPos, ref V2f vel, out bool grounded, out GameObject col)
    {
        grounded = false;
        col = null;

        if(!LevelManager.ready || playerObj is null)
            return;

        GameObject pl = playerObj;

        f32 nl = newPos.x - pl.size.x / 2f;
        f32 nr = newPos.x + pl.size.x / 2f;
        f32 nt = newPos.y + pl.size.y / 2f;
        f32 nb = newPos.y - pl.size.y / 2f;

        foreach(GameObject obj in GameObjectManager.objs)
        {
            if(obj == playerObj)
                continue;

            f32 wl = obj.loc.x - obj.size.x / 2f;
            f32 wr = obj.loc.x + obj.size.x / 2f;
            f32 wt = obj.loc.y + obj.size.y / 2f;
            f32 wb = obj.loc.y - obj.size.y / 2f;

            if(nr > wl && nl < wr)
            {
                if(pl.loc.y + pl.size.y / 2f > wb && pl.loc.y - pl.size.y / 2f < wt)
                {
                    // TODO: portals

                    if(newPos.x > pl.loc.x)
                        newPos.x = wl - pl.size.x / 2f;
                    else if(newPos.x < pl.loc.x)
                        newPos.x = wr + pl.size.x / 2f;

                    vel.x = 0f;
                    col = obj;
                }
            }

            if(nb < wt && nt > wb)
            {
                if(pl.loc.x + pl.size.x / 2f > wl && pl.loc.x - pl.size.x / 2f < wr)
                {
                    if(newPos.y > pl.loc.y)
                        newPos.y = wb - pl.size.y / 2f;
                    else if(newPos.y < pl.loc.y)
                    {
                        newPos.y = wt + pl.size.y / 2f;
                        grounded = true;
                    }

                    vel.y = 0f;
                    col = obj;
                }
            }
        }
    }

    private static bool RectsIntersect(V2i l1, V2i s1, V2i l2, V2i s2)
        => i32.Max(l1.x, l2.x) < i32.Min(l1.x + s1.x, l2.x + s2.x) &&
        i32.Max(l1.y, l2.y) < i32.Min(l1.y + s1.y, l2.y + s2.y);
}