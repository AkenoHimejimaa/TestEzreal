using System;
using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;
using EnsoulSharp.SDK.Rendering;
using SharpDX;

namespace TestEzreal
{
    internal class Ezreal
    {
        public Ezreal()
        {
            GameEvent.OnGameLoad += OnGameLoad;
        }

        private static AIHeroClient Player => ObjectManager.Player;
        private static Menu _ezrealMenu;
        private static Spell _q, _w, _e, _r;
        
        // Menu components
        private static MenuBool _comboQ = new MenuBool("_comboQ", "Use Q on combo");

        private static MenuBool _drawQRange = new MenuBool("_drawQRange", "Draw Q Range");
        private void OnGameLoad()
        {
            if (Player.CharacterName != "Ezreal") return;
            
            Game.Print("Ezreal Loaded"); // -> Print on game chat
            Console.WriteLine("Ezreal Loaded"); // -> Print on console
            
            // Spells
            // For Spell info check on champ wiki

            // new Spell(Slot, range)
            _q = new Spell(SpellSlot.Q, 1200f); 
            // SetSkillshot(delay,width,speed,has collision, Spell Type);
            _q.SetSkillshot(0.25f,60f,2000f,true,SpellType.Line); // -> reducing the width can help improve the pred
            _w = new Spell(SpellSlot.W, 1200f);
            _w.SetSkillshot(0.25f,160f,1700f,false,SpellType.Line);
            _e = new Spell(SpellSlot.E, 475);
            _r = new Spell(SpellSlot.R, float.MaxValue);
            _r.SetSkillshot(1f,320f,2000f,false,SpellType.Line);
            
            // Menu
            _ezrealMenu = new Menu("ezreal", "Ezreal", true);
            var comboMenu = new Menu("combo", "Combo")
            {
                _comboQ
            };
            _ezrealMenu.Add(comboMenu); // -> Add subMenu "combo" to main Menu ezreal

            var drawMenu = new Menu("draw", "Draw")
            {
                _drawQRange
            };
            _ezrealMenu.Add(drawMenu);

            _ezrealMenu.Attach(); // -> To add our menu to Ensoul main Menu;
            
            // Events
            GameEvent.OnGameTick += OnGameTick;
            Drawing.OnDraw += OnDraw;
        }

        private void OnDraw(EventArgs args)
        {
            if (Player.IsDead) return;
            if (_drawQRange.Enabled)
            {
                //CircleRender.Draw(Player.Position,_q.Range,Color.Green,1,true);
                // -> u can also use this drawing style
                Drawing.DrawCircleIndicator(Player.Position,_q.Range,Color.Green.ToSystemColor());
                // Thanks for watching <3
            }
        }

        private void OnGameTick(EventArgs args)
        {
            if (Player.IsDead) return;
            switch (Orbwalker.ActiveMode)
            {
                case OrbwalkerMode.Combo:
                    comboLogic();
                    break;
            }
        }

        private void comboLogic()
        {
            if (_comboQ.Enabled && _q.IsReady())
            {
                var qTarget = TargetSelector.GetTarget(_q.Range, DamageType.Physical);
                // also can be -> var qTarget = _q.GetTarget();
                if (qTarget != null)
                {
                    var qInput = _q.GetPrediction(qTarget);
                    if (qInput.Hitchance >= HitChance.High)
                    {
                        _q.Cast(qInput.CastPosition);
                    }
                }
            }
        }
    }
}