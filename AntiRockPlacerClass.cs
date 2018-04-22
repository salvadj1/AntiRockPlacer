using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fougerite;
using UnityEngine;

namespace AntiRockPlacer
{
    public class AntiRockPlacerClass : Fougerite.Module
    {
        public override string Name { get { return "AntiRockPlacer"; } }
        public override string Author { get { return "Salva/Juli"; } }
        public override string Description { get { return "AntiRockPlacer"; } }
        public override Version Version { get { return new Version("1.0"); } }

        public string red = "[color #B40404]";
        public string blue = "[color #81F7F3]";
        public string green = "[color #82FA58]";
        public string yellow = "[color #F4FA58]";
        public string orange = "[color #FF8000]";
        public string pink = "[color #FA58F4]";
        public string white = "[color #FFFFFF]";

        public int terrainLayer;
        public override void Initialize()
        {
            terrainLayer = UnityEngine.LayerMask.GetMask(new string[] { "Static", "Terrain" });
            Hooks.OnEntityDeployedWithPlacer += OnEntityDeployed;
        }
        public override void DeInitialize()
        {
            Hooks.OnEntityDeployedWithPlacer -= OnEntityDeployed;
        }
        public void OnEntityDeployed(Fougerite.Player pl, Fougerite.Entity e, Fougerite.Player actualplacer)
        {
            if (e.Name.ToLower().Contains("camp") || e.Name.ToLower().Contains("furnace") || e.Name.ToLower().Contains("storage") || e.Name.ToLower().Contains("box") || e.Name.ToLower().Contains("bench") || e.Name.ToLower().Contains("sleeping") || e.Name.ToLower().Contains("stash") || e.Name.ToLower().Contains("ceiling") || e.Name.ToLower().Contains("foundation"))
            {
                var location = new Vector3(e.Location.x, e.Location.y, e.Location.z);
                if (e.Name.ToLower().Contains("foundation"))
                {
                    location = new Vector3(e.Location.x, e.Location.y + 6f, e.Location.z);
                }
                else if (e.Name.ToLower().Contains("bench"))
                {
                    location = new Vector3(e.Location.x, e.Location.y + 1f, e.Location.z);
                }
                if (SearchForRock(location))
                {
                    actualplacer.MessageFrom(Name + Version, red + "Not allowed to build so close/inside a Rock " + white + " Your " + e.Name + " has been destroyed");
                    if (!actualplacer.Admin)
                    {
                        e.Destroy();
                    }
                    else
                    {
                        actualplacer.MessageFrom(Name + Version, green + "Admin Bypass ");
                    }
                }
            }
        }
        public bool SearchForRock(Vector3 loca)
        {
            bool RockFound = false;

            Vector3 Vector3Down = new Vector3(0f, -1f, 0f);
            Vector3 Vector3Up = new Vector3(0f, 1f, 0f);


            var loc = loca;
            Vector3 cachedPosition = loc;
            RaycastHit cachedRaycast;
            cachedPosition.y += 100f;
            Vector3 hit = loc;
            try
            {
                for (int i = 1; i <= 6; i++)
                {
                    if (Physics.Raycast(new Vector3(hit.x, hit.y + i, hit.z), Vector3Up, out cachedRaycast, terrainLayer))
                    {
                        cachedPosition = cachedRaycast.point;
                    }
                    if (!Physics.Raycast(new Vector3(cachedPosition.x, cachedPosition.y + i, cachedPosition.z), Vector3Down, out cachedRaycast, terrainLayer))
                    {
                        continue;
                    }
                    if (!string.IsNullOrEmpty(cachedRaycast.collider.gameObject.name))
                    {
                        continue;
                    }
                    if (cachedRaycast.point.y < loca.y)
                    {
                        continue;
                    }
                    RockFound = true;
                    break;
                }
            }
            catch (Exception ex)
            {
                Logger.Log(Name + " " + Version.ToString() + "[ERROR] " + ex.ToString());
                RockFound = false;
            }
            if (RockFound)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
