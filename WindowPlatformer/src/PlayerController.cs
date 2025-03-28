using System.Linq;
using src.Utility;
using src.LevelSystem;
using src.Gui;
using System.Collections.Generic;

namespace src;

public static class PlayerController
{
    public record class PlayerState(GameObject obj)
    {
        public GameObject obj = obj;
        public V2f vel;
        public f32 timeSinceGrounded;
        public f32 timeSinceJumpAttempt;
        public i32 index;
    }


    public const f32 TIME_SCALE = 0.85f;
    public const f32 GRAVITY = -10f;
    public const f32 STOMP_SPEED = -40f;
    public const f32 JUMP_STRENGTH = 3.25f;
    public const f32 H_ACC = 40f;
    public const f32 H_DAMP = 60f;
    public const f32 MAX_H_SPEED = 2.25f;
    public const f32 ABS_MAX_H_SPEED = 5f;
    public const f32 ABS_MAX_V_SPEED = 8f;
    public const f32 JUMP_VALIDATION_WINDOW = 0.225f;
    public const f32 COYOTE_TIME = 0.175f;

    private static List<PlayerState> playersAtGoal;
    private static bool won;


    public static PlayerState[] playerObjs { get; private set; } = [];
    public static Window[] auraWins { get; private set; } = [];
    public static Window[] nonAuraWins { get; private set; } = [];


    public static void OnLevelLoaded()
    {
        won = false;
        playersAtGoal = [];
        playerObjs = GameObjectManager.objs
            .FindAll(o => o.type == ObjectType.Player)
            .Select(o => new PlayerState(o))
            .ToArray();
        Enumerable.Range(0, playerObjs.Length).Do(i => playerObjs[i].index = i);
        auraWins = [..WindowManager.windows.FindAll(w => w.auraIndex != -1)];
    }

    public static void Tick(f32 dt)
    {
        bool stop = false;
        foreach(PlayerState state in playerObjs)
        {
            if(stop)
                return;

            Tick(dt, state, ref stop);
        }

        foreach(Window aw in auraWins)
        {
            aw.worldLoc = playerObjs[aw.auraIndex].obj.loc;
            aw.UpdateWindowPos();
        }
    }


    private static void Tick(f32 dt, PlayerState state, ref bool stop)
    {
        if(!LevelManager.ready)
            return;

        GameObject playerObj = state.obj;

        f32 unscaledDt = dt;
        dt = f32.Min(dt * TIME_SCALE, 0.01f);

        state.vel.y += GRAVITY * dt;

        bool rInput = Input.KeyHeld(Key.Right, Key.D);
        bool lInput = Input.KeyHeld(Key.Left, Key.A);

        if(rInput)
            state.vel.x += H_ACC * dt;
        if(lInput)
            state.vel.x -= H_ACC * dt;

        if(!rInput && !lInput)
        {
            state.vel.x -= f32.Sign(state.vel.x) * H_DAMP * dt;

            if(f32.Abs(state.vel.x) < 0.3f)
                state.vel.x = 0f;
        }

        state.vel.x = f32.Clamp(state.vel.x, -MAX_H_SPEED, MAX_H_SPEED);

        bool stomping = Input.KeyHeld(Key.Down, Key.S);
        if(stomping)
            state.vel.y += STOMP_SPEED * dt;

        if(Input.KeyDown(Key.W, Key.Up, Key.Space))
            state.timeSinceJumpAttempt = JUMP_VALIDATION_WINDOW;

        if(state.timeSinceGrounded > 0f && state.timeSinceJumpAttempt > 0f)
        {
            state.vel.y = JUMP_STRENGTH;
            state.timeSinceGrounded = 0f;
            state.timeSinceJumpAttempt = 0f;
        }

        if(state.vel.y > 0f && !Input.KeyHeld(Key.W, Key.Up, Key.Space))
            state.vel.y = 0f;

        state.vel.x = f32.Clamp(state.vel.x, -ABS_MAX_H_SPEED, ABS_MAX_H_SPEED);
        state.vel.y = f32.Clamp(state.vel.y, -ABS_MAX_V_SPEED, ABS_MAX_V_SPEED);

        V2f newPos = playerObj.loc + state.vel * dt;

        HandleCollision(playerObj, ref newPos, ref state.vel, out bool grounded, out GameObject col);

        if(grounded)
            state.timeSinceGrounded = COYOTE_TIME;

        state.timeSinceJumpAttempt -= unscaledDt;
        state.timeSinceGrounded -= unscaledDt;

        playerObj.loc = newPos;

        if(col is not null)
            switch(col.type)
            {
                case ObjectType.Danger: LoseLevel(); break;
                case ObjectType.Goal:
                {
                    if(!playersAtGoal.Contains(state))
                        playersAtGoal.Add(state);

                    if(playersAtGoal.Count == playerObjs.Length && !won)
                    {
                        won = true;
                        stop = true;
                        WinLevel();
                        return;
                    }

                    break;
                }
                case ObjectType.Breakable:
                {
                    if(stomping) GameObjectManager.Destroy(col);
                    else
                    {
                        stop = true;
                        LoseLevel();
                        return;
                    }

                    break;
                }
                default: break;
            }

        if(playerObj is not null && !WindowManager.windows.Any(w =>
                w.auraIndex == state.index ||
                RectsIntersect(playerObj.output.GetLoc(), playerObj.output.GetSize(), w.screenLoc, w.screenSize)))
        {
            stop = true;
            LoseLevel();
        }
    }

    private static void LoseLevel()
        => LevelManager.ReloadLevel();

    private static void WinLevel()
        => LevelManager.AdvanceLevel();

    private static void HandleCollision(GameObject playerObj, ref V2f newPos, ref V2f vel, out bool grounded, out GameObject col)
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