﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamTube
{
    interface iDamageable
    {
        int Health { get; set; }
        int DamageTaken { get; set; }
        void TakeDamage(int damage);
        void Death();

    }
}