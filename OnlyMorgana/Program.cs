using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;
using System.Drawing;
using SharpDX;
using Color = System.Drawing.Color;

namespace OnlyMorgana
{   
    internal class Program
    {
        private static string ChampName = "Morgana";
        private static string WelcMsg = "<font color = '#660099'>OnlyMorgana</font><font color='#FFFFFF'> by Da'ath.</font> <font color = '#66ff33'> ~~ LOADED ~~</font>";
        private static int kappa;
        private static int kappaSensei;
        private static Orbwalking.Orbwalker Orbwalker;
        private static Obj_AI_Hero Player;

        private static SpellSlot IgniteSlot;
        private static Spell Q, W, E, R;
        private static List<Spell> Spells = new List<Spell>();

        private static double[] qStun = { 2, 2.25, 2.5, 2.75, 3 };

        private static Items.Item Zhonya;
        private static Menu Menu;

        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            Player = ObjectManager.Player;
            if (Player.BaseSkinName != ChampName) return;
            else Game.PrintChat(WelcMsg);

            SpellsItems();
            LoadMenu();
            Init();
        }

        static void Interrupter_OnPossibleToInterrupt(Obj_AI_Base unit, InterruptableSpell spell)
        {
            if(Menu.Item("inter").GetValue<bool>())
            {
                if (unit.IsValidTarget(Q.Range) && Q.IsReady())
                    Q.CastIfHitchanceEquals(unit, HitChance.High, Packets());
            }
            else
                return;
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if(Menu.Item("HarassActive").GetValue<KeyBind>().Active)
                Harass();
            if (Menu.Item("ComboActive").GetValue<KeyBind>().Active)
                Combo();
            if (Menu.Item("LaneClearActive").GetValue<KeyBind>().Active)
                LaneClear();
            if (Menu.Item("ks").GetValue<bool>())
                SmartKS();
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            //if(sender.IsMe)
            //{
               // Game.PrintChat(args.SData.Name);
            //}

            if (args.SData.Name == "SoulShackles" && sender.IsMe)
            {
                Game.PrintChat("kappa");
                kappaSensei = Environment.TickCount + 5000;
            }

            if (Menu.Item("zhonya").GetValue<bool>() && Zhonya.IsReady())
            {
                //Game.PrintChat("znhony");
                if (Environment.TickCount < kappaSensei)
                {
                    //Game.PrintChat("ticki");
                    if (args.Target.IsMe || Player.Distance(args.End) < 300f)
                    {
                        //Game.PrintChat("IsME");
                        if (Player.Health < GetPlayerHP(Menu.Item("hpZhonya").GetValue<Slider>().Value))
                        {
                            Zhonya.Cast();
                            //Game.PrintChat("puszczam zhonye");
                        }
                    }
                }
            }

            if (E.IsReady() && sender.Type == GameObjectType.obj_AI_Hero && sender.IsEnemy)
            {
                var champList = ObjectManager.Get<Obj_AI_Hero>().Where(f => f.IsAlly && !f.IsDead).OrderBy(f => f.Distance(args.End));
                if (Menu.Item("enable").GetValue<bool>())
                {
                    foreach (var champ in champList)
                    {
                        foreach (var spell in SpellList.CCList)
                        {
                           //Game.PrintChat(args.SData.Name);
                            if (args.SData.Name == spell.SDataName)
                            {
                                if (Menu.Item(spell.SDataName).GetValue<bool>())
                                {
                                    switch (spell.Type)
                                    {
                                        case Skilltype.Unknown:
                                            if (champ.IsMe)
                                            {
                                                if (champ.Distance(sender, true) < 600f)
                                                {
                                                    E.CastOnUnit(champ, Packets());
                                                }
                                            }
                                            else if (Menu.Item("useOn" + champ.BaseSkinName).GetValue<bool>())
                                            {
                                                if (champ.Distance(sender, true) < 600f)
                                                {
                                                    if (Player.Distance(champ, true) < E.Range)
                                                    {
                                                        E.CastOnUnit(champ, Packets());
                                                    }
                                                }
                                            }
                                            break;
                                        case Skilltype.Line:
                                            if (champ.IsMe)
                                            {
                                                if (champ.Distance(args.End) < 300f)
                                                {
                                                    E.CastOnUnit(champ, Packets());
                                                }
                                            }
                                            else if (Menu.Item("useOn" + champ.BaseSkinName).GetValue<bool>())
                                            {
                                                if (champ.Distance(args.End) < 300f)
                                                {
                                                    if (Player.Distance(champ, true) < E.Range)
                                                    {
                                                        E.CastOnUnit(champ, Packets());
                                                    }
                                                }
                                            }
                                            break;
                                        case Skilltype.Cone:
                                            if (champ.IsMe)
                                            {
                                                if (champ.Distance(args.End) < 150f)
                                                {
                                                    E.CastOnUnit(champ, Packets());
                                                }
                                            }
                                            else if (Menu.Item("useOn" + champ.BaseSkinName).GetValue<bool>())
                                            {
                                                if (champ.Distance(args.End) < 150f)
                                                {
                                                    if (Player.Distance(champ, true) < E.Range)
                                                    {
                                                        E.CastOnUnit(champ, Packets());
                                                    }
                                                }
                                            }
                                            break;
                                        case Skilltype.Circle:
                                            if (champ.IsMe)
                                            {
                                                if (champ.Distance(args.End) < 250f)
                                                {
                                                    E.CastOnUnit(champ, Packets());
                                                }
                                            }
                                            else if (Menu.Item("useOn" + champ.BaseSkinName).GetValue<bool>())
                                            {
                                                if (champ.Distance(args.End) < 250f)
                                                {
                                                    if (Player.Distance(champ, true) < E.Range)
                                                    {
                                                        E.CastOnUnit(champ, Packets());
                                                    }
                                                }
                                            }
                                            break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (gapcloser.Sender.IsAlly)
                return;

            if (Q.IsReady() && Menu.Item("qGap").GetValue<bool>())
                Q.CastIfHitchanceEquals(gapcloser.Sender, HitChance.High, Packets());
            else return;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            foreach(var spell in Spells)
            {
                if (Menu.Item("Draw" + spell.Slot.ToString().ToLower()).GetValue<Circle>().Active)
                {
                    if (spell.IsReady())
                        Utility.DrawCircle(Player.ServerPosition, spell.Range, Menu.Item("Draw" + spell.Slot.ToString().ToLower()).GetValue<Circle>().Color);
                    else
                        Utility.DrawCircle(Player.ServerPosition, spell.Range, Color.Red);
                }
            }

            foreach(var enemy in ObjectManager.Get<Obj_AI_Hero>().Where(f => f.IsVisible && !f.IsDead && !f.IsAlly && f.Health < GetComboDamage(f)))
            {
                Utility.DrawCircle(enemy.Position, 50f, Color.Red);
            }

        }

        private static void Combo()
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (target == null) return;

            //Game.PrintChat("COMBO");

            if (Menu.Item("qCombo").GetValue<bool>() && Q.IsReady() && target.IsValidTarget(Q.Range))
            {
                Q.CastIfHitchanceEquals(target, GetHitChance(), Packets());
                //Game.PrintChat("q COMBO !!!!!");
            }
            if (Menu.Item("wCombo").GetValue<bool>() && W.IsReady() && target.IsValidTarget(W.Range))
            {
                if(Menu.Item("w2Combo").GetValue<bool>())
                {
                    if (target.HasBuffOfType(BuffType.Fear) || target.HasBuffOfType(BuffType.Sleep) || target.HasBuffOfType(BuffType.Snare)
                        || target.HasBuffOfType(BuffType.Stun) || target.HasBuffOfType(BuffType.Taunt))
                            W.Cast(target.Position, Packets());
                }
                else
                {
                    W.CastIfHitchanceEquals(target, HitChance.High, Packets());
                }
            }
            if (Menu.Item("rCombo").GetValue<bool>() && R.IsReady() && target.IsValidTarget(R.Range))
            {
                if(GetComboDamage(target) > target.Health)
                {
                    R.Cast(Packets());
                }
            }
            if(Menu.Item("rifComboActive").GetValue<bool>() && R.IsReady())
            {
                if(Utility.CountEnemysInRange(Player, (int)R.Range) >= kappa)
                {
                    R.Cast(Packets());
                }
            }
            if (IgniteSlot != SpellSlot.Unknown && Player.Spellbook.CanUseSpell(IgniteSlot) == SpellState.Ready && Menu.Item("ignite").GetValue<bool>() && GetComboDamage(target) > target.Health)
            {
                Player.Spellbook.CastSpell(IgniteSlot, target);
            }

        }

        private static void SmartKS()
        {
            var enemies1 = ObjectManager.Get<Obj_AI_Hero>().Where(f => !f.IsAlly && !f.IsDead && f.IsValidTarget(Q.Range));
            if (enemies1 == null) return;

            foreach (var target in enemies1)
            {
                if (Player.GetSpellDamage(target, SpellSlot.Q) > target.Health + 150 && Q.IsReady())
                {
                    Q.CastIfHitchanceEquals(target, HitChance.High, Packets());
                    //Game.PrintChat("Q KS" + target.BaseSkinName);
                }
                if((Player.GetSpellDamage(target, SpellSlot.W) * 3) > target.Health + 50 && target.IsValidTarget(W.Range) && W.IsReady())
                {
                    W.CastIfHitchanceEquals(target, HitChance.High, Packets());
                    //Game.PrintChat("W KS " + target.BaseSkinName);
                }
            }
        }

        private static void Harass()
        {
            if (Menu.Item("mpHarassActive").GetValue<bool>())
                if (Player.Mana < GetPlayerMana(Menu.Item("mpHarass").GetValue<Slider>().Value))
                    return;

            var enemies2 = ObjectManager.Get<Obj_AI_Hero>().Where(f => !f.IsAlly && !f.IsDead && f.IsValidTarget(Q.Range));
            if (enemies2 == null) return;

            foreach(var target in enemies2)
            {
                if (Menu.Item("qHarass").GetValue<bool>() && Q.IsReady() && Q.GetPrediction(target).Hitchance >= HitChance.VeryHigh)
                {
                    Q.CastIfHitchanceEquals(target, HitChance.VeryHigh, Packets());
                    //Game.PrintChat("q harass");
                }
                if (Menu.Item("wHarass").GetValue<bool>() && target.IsValidTarget(W.Range) && W.IsReady())
                {
                    if (target.HasBuffOfType(BuffType.Fear) || target.HasBuffOfType(BuffType.Sleep) || target.HasBuffOfType(BuffType.Snare)
                        || target.HasBuffOfType(BuffType.Stun) || target.HasBuffOfType(BuffType.Taunt))
                    {
                        W.Cast(target.Position, Packets());
                        //Game.PrintChat("w harass");
                    }
                }

            }
        }

        private static void LaneClear()
        {
            if (Menu.Item("mpLaneClearActive").GetValue<bool>())
                if (Player.Mana < GetPlayerMana(Menu.Item("mpLaneClear").GetValue<Slider>().Value))
                    return;

            var minions = MinionManager.GetMinions(Player.Position, Q.Range);
            /*foreach( var kupa in minions)
            {
                Game.PrintChat(kupa.Name);
            }*/
            if (minions == null) return;
            if(Menu.Item("qLaneClear").GetValue<bool>() && Q.IsReady())
            {
                var minionsQ = MinionManager.GetMinions(Player.ServerPosition, Q.Range, MinionTypes.All,
                            MinionTeam.NotAlly, MinionOrderTypes.MaxHealth);
                    
                foreach(var vMinion in
                                    from vMinion in minionsQ
                                    let vMinionQDamage = Player.GetSpellDamage(vMinion, SpellSlot.Q)
                                    where vMinion.Health <= vMinionQDamage && vMinion.Health > Player.GetAutoAttackDamage(vMinion)
                                    select vMinion)
                {
                    if (vMinion.BaseSkinName.Contains("Siege") || vMinion.BaseSkinName.Contains("Super"))
                    {
                        //Game.PrintChat("minions");
                        Q.CastIfHitchanceEquals(vMinion, HitChance.VeryHigh);
                    }
                }
            }

            if(Menu.Item("wLaneClear").GetValue<bool>() && W.IsReady())
            {
                var minionsW = MinionManager.GetBestCircularFarmLocation(minions.Select(minion => minion.ServerPosition.To2D()).ToList(), 350f, W.Range);
                    if(minionsW.MinionsHit >= 2)
                    {
                        W.Cast(minionsW.Position, Packets());
                    }
            }
        }
        private static void SpellsItems()
        {
            Q = new Spell(SpellSlot.Q, 1075f);
            W = new Spell(SpellSlot.W, 850f);
            E = new Spell(SpellSlot.E, 700f);
            R = new Spell(SpellSlot.R, 600f);
            Spells.AddRange(new[] { Q, W, E, R });

            Q.SetSkillshot(250f, 80f, 1200f, true, SkillshotType.SkillshotLine);
            W.SetSkillshot(0.25f, 350f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            E.SetTargetted(0.25f, float.MaxValue);

            IgniteSlot = Player.GetSpellSlot("SummonerDot");

            Zhonya = new Items.Item((int)ItemId.Zhonyas_Hourglass, 0f);
        }
        private static float GetComboDamage(Obj_AI_Base target)
        {
            double dmg = 0;

            if (Q.IsReady() && Menu.Item("qCombo").GetValue<bool>())
            {
                dmg += dmg + Player.GetSpellDamage(target, SpellSlot.Q);
            }

            if(W.IsReady() && Menu.Item("wCombo").GetValue<bool>())
            {
                if (target.HasBuffOfType(BuffType.Snare))
                {
                   double stun = qStun[Q.Level] / 0.5;
                    dmg += Player.GetSpellDamage(target, SpellSlot.W) * stun;
                }
               else
                   dmg += Player.GetSpellDamage(target, SpellSlot.W) * 2;
            }
            if(R.IsReady() && Menu.Item("rCombo").GetValue<bool>())
            {
                if (target.HasBuffOfType(BuffType.Fear) || target.HasBuffOfType(BuffType.Sleep) || target.HasBuffOfType(BuffType.Snare)
                        || target.HasBuffOfType(BuffType.Stun) || target.HasBuffOfType(BuffType.Taunt))
                {
                    dmg += Player.GetSpellDamage(target, SpellSlot.R) * 2;
                }
                else
                    dmg += Player.GetSpellDamage(target, SpellSlot.R);
            }
            if (IgniteSlot != SpellSlot.Unknown && Player.Spellbook.CanUseSpell(IgniteSlot) == SpellState.Ready && Menu.Item("ignite").GetValue<bool>())
                dmg += ObjectManager.Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);

            return (float)dmg;

        }
        private static float GetPlayerMana(float mp)
        {
            return Player.MaxMana * (mp / 100);
        }

        private static HitChance GetHitChance()
        {
            HitChance hitC = HitChance.Medium;

            int i = Menu.Item("qHitChance").GetValue<StringList>().SelectedIndex;

            switch (i)
            {
                case 0:
                    hitC = HitChance.Medium;
                    break;
                case 1:
                    hitC = HitChance.High;
                    break;
                case 2:
                    hitC = HitChance.VeryHigh;
                    break;
            }

            return hitC;
        }
        private static float GetPlayerHP(float hp)
        {
            return Player.MaxHealth * (hp / 100);
        }

        private static void LoadMenu()
        {
                Menu = new Menu("OnlyMorgana", ChampName, true);

                var orbwalkerMenu = new Menu("My orbwalker", "orbwalkerMenu");
                Menu.AddSubMenu(orbwalkerMenu);
                Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);

                var tsMenu = new Menu("Target Selector", "tsMenu");
                TargetSelector.AddToMenu(tsMenu);
                orbwalkerMenu.AddSubMenu(tsMenu);

                var Combo = new Menu("Combo", "Combo");
                Menu.AddSubMenu(Combo);
                Combo.AddItem(new MenuItem("qCombo", " Use Q").SetValue(true));
                Combo.AddItem(new MenuItem("qHitChance", "Q HitChance").SetValue(new StringList(new[] { "Low", "Medium", "High" })));
                Combo.AddItem(new MenuItem("wCombo", "Use W").SetValue(true));
                Combo.AddItem(new MenuItem("w2Combo", "W only on immovable").SetValue(true));
                Combo.AddItem(new MenuItem("rCombo", "Use R").SetValue(true));
                var kappaHD = Combo.AddItem(new MenuItem("rif", "R if X enemys").SetValue(new Slider(2, 1, 5)));
                kappa = Menu.Item("rif").GetValue<Slider>().Value;
                int lastTick = Environment.TickCount;
                kappaHD.ValueChanged += delegate(object sender, OnValueChangeEventArgs EventArgs)
                {
                    if(Environment.TickCount > lastTick)
                    {
                        Game.PrintChat("Value changed, press F5 to reload.");
                        lastTick = Environment.TickCount + 5000;
                    }
                };
                Combo.AddItem(new MenuItem("rifComboActive", "R if " + kappa + " enemys").SetValue(true));
                Combo.AddItem(new MenuItem("zhonya", "Use Zhonya").SetValue(true));   
                Combo.AddItem(new MenuItem("hpZhonya", "Zhonya HP").SetValue(new Slider(70, 1, 100)));              
                Combo.AddItem(new MenuItem("ignite", "Use Ignite").SetValue(true));
                Combo.AddItem(new MenuItem("ComboActive", "Active").SetValue(new KeyBind(32, KeyBindType.Press)));

                var Harass = new Menu("Harass", "Harass");
                Menu.AddSubMenu(Harass);
                Harass.AddItem(new MenuItem("qHarass", "Use Q").SetValue(true));
                Harass.AddItem(new MenuItem("wHarass", "Use W").SetValue(true));
                Harass.AddItem(new MenuItem("mpHarass", "Mana limit:").SetValue(new Slider(30, 1, 100)));
                Harass.AddItem(new MenuItem("mpHarassActive", "Mana limiter active").SetValue(true));
                Harass.AddItem(new MenuItem("HarassActive", "Active (toggle)").SetValue(new KeyBind("Y".ToArray()[0], KeyBindType.Toggle)));

                var LaneClear = new Menu("Lane Clear", "LaneClear");
                Menu.AddSubMenu(LaneClear);
                LaneClear.AddItem(new MenuItem("qLaneClear", "Use Q").SetValue(true));
                LaneClear.AddItem(new MenuItem("wLaneClear", "Use W").SetValue(true));
                LaneClear.AddItem(new MenuItem("mpLaneClear", "Mana limit:").SetValue(new Slider(30, 1, 100)));
                LaneClear.AddItem(new MenuItem("mpLaneClearActive", "Mana limiter active").SetValue(true));
                LaneClear.AddItem(new MenuItem("LaneClearActive", "Active").SetValue(new KeyBind("V".ToArray()[0], KeyBindType.Press)));

                var Drawings = new Menu("Drawings", "Drawings");
                Menu.AddSubMenu(Drawings);
                Drawings.AddItem(new MenuItem("Drawq", "Draw Q").SetValue(new Circle(true, Color.FromArgb(100, 255, 0, 255))));
                Drawings.AddItem(new MenuItem("Draww", "Draw W").SetValue(new Circle(true, Color.FromArgb(100, 255, 0, 255))));
                Drawings.AddItem(new MenuItem("Drawe", "Draw E").SetValue(new Circle(true, Color.FromArgb(100, 255, 0, 255))));
                Drawings.AddItem(new MenuItem("Drawr", "Draw R").SetValue(new Circle(true, Color.FromArgb(100, 255, 0, 255))));
                Drawings.AddItem(new MenuItem("DrawKill", "Draw killable").SetValue(true));

                var Misc = new Menu("Misc", "Misc");
                Menu.AddSubMenu(Misc);
                Misc.AddItem(new MenuItem("packets", "Use Packets").SetValue(true));
                Misc.AddItem(new MenuItem("ks", "Smart KS").SetValue(true));
                Misc.AddItem(new MenuItem("qGap", "Use Q on gapcloser").SetValue(true));
                Misc.AddItem(new MenuItem("inter", "Auto Interrupt Spells").SetValue(true));
                var EMenu = new Menu("Shield Menu", "EMenu");
                Misc.AddSubMenu(EMenu);
                EMenu.AddItem(new MenuItem("enable", "Enable").SetValue(true));

                var suppSpells = new Menu("Supported Spells:", "spells");
                foreach (Obj_AI_Hero enemies in ObjectManager.Get<Obj_AI_Hero>().Where(f => !f.IsAlly))
                {
                    foreach (var spell in SpellList.CCList)
                    {
                        if (enemies.BaseSkinName == spell.HeroName)
                        {
                            suppSpells.AddItem(new MenuItem(spell.SDataName, enemies.BaseSkinName + " " + spell.Slot.ToString()).SetValue(true));
                        }
                    }
                }
                EMenu.AddSubMenu(suppSpells);

                var useOn = new Menu("Use on:", "useOn");
                foreach (Obj_AI_Hero ally in ObjectManager.Get<Obj_AI_Hero>().Where(f => f.IsAlly))
                {
                        useOn.AddItem(new MenuItem("useOn" + ally.BaseSkinName, ally.BaseSkinName).SetValue(true));
                }
                EMenu.AddSubMenu(useOn);

                Menu.AddToMainMenu();
        }
        private static void Init()
        {
            Drawing.OnDraw += Drawing_OnDraw;
            Interrupter.OnPossibleToInterrupt += Interrupter_OnPossibleToInterrupt;
            Game.OnGameUpdate += Game_OnGameUpdate;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
        }
        private static bool Packets()
        {
            return Menu.Item("packets").GetValue<bool>();
        }

    }
}
