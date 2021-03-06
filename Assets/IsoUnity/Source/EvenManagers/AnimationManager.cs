﻿using UnityEngine;
using System.Collections;
using IsoUnity;

namespace IsoUnity.Entities
{
    public class AnimationManager : EventManager
    {

        public override void ReceiveEvent(IGameEvent ev)
        {
            if (ev.Name == "ShowAnimation")
            {
                Decoration dec = (ev.getParameter("Objective") as GameObject).GetComponent<Decoration>();
                GameObject animation = (GameObject)ev.getParameter("Animation");

                GameObject go = (GameObject)GameObject.Instantiate(animation);

                Decoration animation2 = go.GetComponent<Decoration>();

                animation2.GetComponent<Renderer>().sharedMaterial = new Material(Shader.Find("Transparent/Cutout/Diffuse"));
                animation2.Father = dec;
                animation2.adaptate();

                AutoAnimator anim = go.GetComponent<AutoAnimator>();
                anim.registerEvent(ev);
            }

            if (ev.Name == "show decoration animation")
            {
                Decoration dec = (ev.getParameter("objective") as GameObject).GetComponent<Decoration>();

                GameObject go = GameObject.CreatePrimitive(PrimitiveType.Quad);
                Decoration decoration = go.AddComponent<Decoration>();
                decoration.IsoDec = (IsoDecoration)ev.getParameter("animation");

                decoration.GetComponent<Renderer>().sharedMaterial = new Material(Shader.Find("Transparent/Cutout/Diffuse"));
                decoration.Father = dec;
                decoration.Centered = true;
                decoration.adaptate();
                decoration.SendMessage("Update");

                AutoAnimator anim = go.AddComponent<AutoAnimator>();
                anim.FrameSecuence = new int[8] { 0, 1, 2, 3, 4, 5, 6, 7 };
                anim.FrameRate = 0.07f;
                anim.AutoDestroy = true;
                anim.Repeat = 1;
                anim.registerEvent(ev);
            }
        }

        public override void Tick() { }

    }
}