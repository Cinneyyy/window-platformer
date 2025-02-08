using System.Linq;
using System.Numerics;

namespace src;

public static class PlayerController
{
    public const f32 GRAVITY = -10f;
    public const f32 STOMP_SPEED = -40f;
    public const f32 JUMP_STRENGTH = 3.25f;
    public const f32 H_ACC = 40f;
    public const f32 H_DAMP = 60f;
    public const f32 MAX_H_SPEED = 2.25f;
    public const f32 COYOTE_TIME = 0.1f;
    public const f32 JUMP_TOLERANCE = 0.1f;

    private static bool grounded;
    private static float jumpTimer;
    private static V2f vel;


    public static void Tick(f32 dt)
    {
        if(!GameState.isLevelLoaded)
        {
            vel = V2f.zero;
            grounded = false;
            return;
        }

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

        if(Input.KeyHeld(Key.W, Key.Up, Key.Space))
            jumpTimer = JUMP_TOLERANCE;
        else
            jumpTimer -= dt;

        if(grounded && jumpTimer > 0f)
        {
            vel.y = JUMP_STRENGTH;
            jumpTimer = 0f;
            grounded = false;
        }

        // TODO: stronger gravity if not holding space (jump ctrl)

        V2f newPos = GameState.player!.loc + vel * dt;

        HandleCollision(ref newPos, ref vel, out grounded, out GameObject? col);

        GameState.player.loc = newPos;
        GameState.player.OnMove();

        if(col is not null)
            switch(col.type)
            {
                case ObjectType.Danger:
                {
                    GameState.KillPlayer();
                    break;
                }
                case ObjectType.Goal:
                {
                    GameState.WinLevel();
                    break;
                }
                case ObjectType.Breakable:
                {
                    if(stomping)
                        col.enabled = false;
                    else
                        GameState.KillPlayer();

                    break;
                }
                default: break;
            }

        if(!GameState.windows!.Any(w => RectsIntersect(GameState.player.loc, GameState.player.size, w.worldLoc, w.worldSize)))
            GameState.KillPlayer();
    }


    private static void HandleCollision(ref V2f newPos, ref V2f vel, out bool grounded, out GameObject? col)
    {
        grounded = false;
        col = null;

        if(!GameState.isLevelLoaded)
            return;

        GameObject pl = GameState.player!;

        f32 nl = newPos.x - pl.size.x / 2f;
        f32 nr = newPos.x + pl.size.x / 2f;
        f32 nt = newPos.y + pl.size.y / 2f;
        f32 nb = newPos.y - pl.size.y / 2f;

        for(int i = 0; i < GameState.objects!.Length; i++)
        {
            GameObject obj = GameState.objects[i];

            if(!obj.enabled)
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

    private static bool RectsIntersect(V2f l1, V2f s1, V2f l2, V2f s2)
        => f32.Max(l1.x, l2.x) < f32.Min(l1.x + s1.x, l2.x + s2.x) &&
        f32.Max(l1.y, l2.y) < f32.Min(l1.y + s1.y, l2.y + s2.y);
}