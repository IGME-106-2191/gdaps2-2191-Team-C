﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamTube
{
    interface iMovable
    {
        void GetAdjacent(TileType[] tiles, int[] characters);
    }
}