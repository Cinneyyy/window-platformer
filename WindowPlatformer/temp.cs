using src;

if(entryAnim)
{
    u32 animStart = SDL_GetTicks();
    V2f[] startPos = wins.Select(w => w.worldLoc).ToArray();
    V2f[] startSize = wins.Select(w => w.worldSize).ToArray();

    while(SDL_GetTicks() - animStart is u32 timePassed && timePassed < ENTRY_ANIM_TIME)
    {
        f32 t = Easing.Out.Cube((f32)timePassed / ENTRY_ANIM_TIME);

        for(i32 i = 0; i < data.Length; i++)
        {
            if(data[i].entrySize == V2f.one && data[i].entryDir == V2f.zero)
                continue;

            if(data[i].entrySize != V2f.one)
            {
                wins[i].worldSize = V2f.Lerp(startSize[i], data[i].size, t);
                wins[i].worldLoc = wins[i].worldLoc;
                wins[i].UpdateWindowSize();
            }

            if(data[i].entryDir != V2f.zero)
                wins[i].worldLoc = V2f.Lerp(startPos[i], data[i].loc, t);

            wins[i].UpdateWindowPos();

            if(wins[i].exitRedraw)
                Renderer.DrawWindow(wins[i]);
            //Renderer.DrawWindow(wins[i], Screen.WorldPointToScreen(data[i].loc + new V2f(-wins[i].worldSize.x/2f, wins[i].worldSize.y/2f)));
        }

        SDL_Delay(WINDOW_ANIM_DT);
    }

    for(i32 i = 0; i < data.Length; i++)
    {
        wins[i].worldSize = data[i].size;
        wins[i].worldLoc = data[i].loc;

        wins[i].UpdateWindowPos();

        if(data[i].entrySize != V2f.one)
        {
            wins[i].UpdateWindowSize();
            //wins[i].RecreateRenderer();
        }
    }
}