using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;
using System;
using System.Collections.Generic;
using Jypeli.Effects;


/// Funktio ja taulukko: https://tim.jyu.fi/view/kurssit/tie/ohj1/2021k/demot/demo11?answerNumber=5&b=fjxau62oK3ZY&size=1&task=taulukot&user=miljohsa#taulukot
/// Silmukka: https://tim.jyu.fi/view/kurssit/tie/ohj1/2021k/demot/demo5?answerNumber=2&b=lVmLFdjIUEDn&size=1&task=lukujenTulostusT2&user=miljohsa

/// @author Milla
/// version 25.4.2021
/// <summary>
/// Lentokonepeli, Ohjelmointi 1 -kurssille harjoitustyönä tehty peli.
/// </summary>



public class Lentokonepeli : PhysicsGame
{
    /// <summary>
    /// Pelikentän taustakuva.
    /// </summary>
    private Image taustakartta = LoadImage("harkkapeli_kartta");

    /// <summary>
    /// Lentokone, jota pelaaja liikuttaa hiirellä.
    /// </summary>
    private PhysicsObject lentokone;

    /// <summary>
    /// Pelin pistelaskuri.
    /// </summary>
    private IntMeter pisteLaskuri;

    /// <summary>
    /// Pelin aikalaskuri.
    /// </summary>
    private Timer laskuri;

    /// <summary>
    /// Pelin laskeva aikalaskuri.
    /// </summary>
    private DoubleMeter laskuriAlaspain;

    /// <summary>
    /// Pelissä kerättävien asioiden määrä.
    /// </summary>
    private const int KERATTAVIEN_MAARA = 11;


    public override void Begin()
    {

        ClearAll();
        AloitusValikko();

    }


    /// <summary>
    /// Luo pelikentän ja määrittelee sen ominaisuudet.
    /// </summary>
    private void LuoKentta()
    {
        Level.Size = new Vector(1000, 1000);
        SetWindowSize(900, 800);
        Camera.ZoomToLevel();
        Level.Background.Image = taustakartta;
        Mouse.IsCursorVisible = true;
        Vector paikkaRuudulla = Mouse.PositionOnScreen;
    }


    /// <summary>
    /// Luo lentokoneen.
    /// </summary>
    private void Lentokone()
    {
        lentokone = new PhysicsObject(50, 50);
        lentokone.CanRotate = true;
        lentokone.Image = LoadImage("lentskari2");
        Add(lentokone);

        Mouse.ListenMovement(0.1, LiikutaLentokonetta, "Lennä", lentokone);
    }


    /// <summary>
    /// Lentokonene liikutus. Pelajaa liikutta lentokonetta hiirellään.
    /// </summary>
    /// <param name="lentokone">liikutettava lentokone</param>
    private void LiikutaLentokonetta(PhysicsObject lentokone)
    {
        lentokone.Position = Mouse.PositionOnWorld;
    }


    /// <summary>
    /// Luo pistelaskurin peliin.
    /// </summary>
    private void LuoPistelaskuri()
    {
        pisteLaskuri = new IntMeter(0);

        Label pisteNaytto = new Label();
        pisteNaytto.X = Screen.Left + 100;
        pisteNaytto.Y = Screen.Top - 100;
        pisteNaytto.TextColor = Color.Black;
        pisteNaytto.Color = Color.White;
        pisteNaytto.Title = "Pisteet";

        pisteNaytto.BindTo(pisteLaskuri);
        Add(pisteNaytto);
    }


    /// <summary>
    /// Luo aikalaskurin.
    /// </summary>
    private void LuoLaskuri()
    {
        laskuriAlaspain = new DoubleMeter(25);

        laskuri = new Timer();
        laskuri.Interval = 0.1;
        laskuri.Timeout += LaskeAlaspain;
        laskuri.Start();

        Label Naytto = new Label();
        Naytto.TextColor = Color.White;
        Naytto.DecimalPlaces = 1;
        Naytto.X = Screen.Left + 100;
        Naytto.Y = Screen.Top - 150;
        Naytto.BindTo(laskuriAlaspain);
        Add(Naytto);
    }


 /// <summary>
 /// Luo laskevan aikalaskurin
 /// </summary>
    private void LaskeAlaspain()
    {
        laskuriAlaspain.Value -= 0.1;
        if (laskuriAlaspain.Value <= 0)
        {
            MessageDisplay.Add("Aika loppui, pisteesi: " + pisteLaskuri);
            laskuri.Stop();

        }
    }


    /// <summary>
    /// Lisää kerättävan asian peliin.
    /// </summary>
    private void LisaaKerattava()
    {

        int i = 1;
        while (i < KERATTAVIEN_MAARA)
        {
            PhysicsObject PunainenKerattava = new PhysicsObject(20, 20);
            PunainenKerattava.Shape = Shape.Heart;
            PunainenKerattava.Color = Color.Red;
            PunainenKerattava.CollisionIgnoreGroup = 1;
            PunainenKerattava.Tag = "Sydän";
            Direction suunta = RandomGen.NextDirection();
            Add(PunainenKerattava);
            PunainenKerattava.Position = RandomGen.NextVector(Level.BoundingRect);
            i++;
        }

        AddCollisionHandler(lentokone, "Sydän", PelaajaKerasi);
    }


    /// <summary>
    /// Lisää pisteen ja tuhoaa kerättävän asian lentokoneen osuessa siihen.
    /// </summary>
    /// <param name="lentokone">Pelaaja</param>
    /// <param name="PunainenKerattava">Kerättävä asia</param>
    private void PelaajaKerasi(PhysicsObject lentokone, PhysicsObject PunainenKerattava)
    {
        pisteLaskuri.Value += 1;
        PunainenKerattava.Destroy();
    }


    /// <summary>
    /// Lisää ukkospilviä peliin.
    /// </summary>
    private void LisaaUkkonen()
    {
        Timer ukkonen = new Timer();
        ukkonen.Interval = 1.5;
        ukkonen.Timeout += LuoUkkonen;
        ukkonen.Start();
    }


    /// <summary>
    /// Määrittelee ukkospilvien ominaisuudet.
    /// </summary>
    private void LuoUkkonen()
    {
        PhysicsObject salama = new PhysicsObject(80, 80);
        salama.Image = LoadImage("ukkospilvi2");
        salama.CollisionIgnoreGroup = 1;
        Direction suunta = RandomGen.NextDirection();
        salama.Mass = 10.0;
        salama.AngularVelocity = 0.0;
        salama.Oscillate(Vector.UnitY, 400, 0.01);

        double kokeilux = RandomGen.NextDouble(Level.Left, Level.Right);
        salama.Tag = "Ukonilma";
        Add(salama);
        salama.Position = RandomGen.NextVector(Level.BoundingRect);

        AddCollisionHandler(lentokone, salama, PelaajaTormasiUkkoseen);
    }


    /// <summary>
    /// Lisää pyörremyrskyjä peliin.
    /// </summary>
    private void LisaaPyorremyrsky()
    {
        int i = 1;
        while (i < 20)
        {
            PhysicsObject Pyorremyrsky = new PhysicsObject(80, 80);
            Pyorremyrsky.Image = LoadImage("pyörremyrsky1");
            Pyorremyrsky.CollisionIgnoreGroup = 1;
            Direction suunta = RandomGen.NextDirection();
            Pyorremyrsky.Oscillate(Vector.UnitX, 400, 0.007);
            ///hurrikaani1.Oscillate(Vector.UnitX, 100, 2, 0.5 * Math.PI);
            Pyorremyrsky.Tag = "Pyorremyrsky";
            Add(Pyorremyrsky);
            Pyorremyrsky.Position = RandomGen.NextVector(Level.BoundingRect);
            i++;

            AddCollisionHandler(lentokone, Pyorremyrsky, PelaajaTormasiPyorremyrskyyn);
        }
    }


    /// <summary>
    /// Keskeyttää pelin lentokoneen osuessa pyörremyrskyyn.
    /// </summary>
    /// <param name="lentokone">Pelaaja</param>
    /// <param name="Pyorremyrsky">Tuhoaa lentokoneen</param>
    private void PelaajaTormasiPyorremyrskyyn(PhysicsObject lentokone, PhysicsObject Pyorremyrsky)
    {
        Pause();
        TormaysPyorremyrskyyn();

    }


    /// <summary>
    /// Vähentää pisteitä pelaajan osuessa lentokoneella ukkospilveen.
    /// </summary>
    /// <param name="lentokone">Pelaaja</param>
    /// <param name="salama">Vähentää pisteitä</param>
    private void PelaajaTormasiUkkoseen(PhysicsObject lentokone, PhysicsObject salama)
    {

        salama.Destroy();
        pisteLaskuri.Value -= 1;
        MessageDisplay.Add("Törmäsit, pisteesi: " + pisteLaskuri);
    }


    /// <summary>
    /// Aloitusvalikon luominen
    /// </summary>
    private void AloitusValikko()
    {
        MultiSelectWindow alkuValikko = new MultiSelectWindow("Tervetuloa pelaamaan lentokonepeliä!",
        "Aloita peli", "Lopeta");
        Level.Background.Image = taustakartta;
        Add(alkuValikko);
        Mouse.IsCursorVisible = true;
        alkuValikko.AddItemHandler(0, AloitaPeli);
        alkuValikko.AddItemHandler(1, Exit);
    }


    /// <summary>
    /// Aloittaa pelin.
    /// </summary>
    private void AloitaPeli()
    {
        ClearAll();
        LuoKentta();
        Lentokone();
        LuoPistelaskuri();
        LuoLaskuri();
        LisaaKerattava();
        LisaaPyorremyrsky();
        LisaaUkkonen();
    }


    /// <summary>
    /// Keskeyttää pelin ja luo valikon, josta pelin voi aloittaa uudestaan.
    /// </summary>
    private void TormaysPyorremyrskyyn()
    {
        Pause();
        MultiSelectWindow alkuValikko = new MultiSelectWindow(
        "Törmäsit pyörremyrskyyn!", "Pelaa uudestaan!", "Lopeta");
        Level.Background.Image = taustakartta;
        Add(alkuValikko);
        Mouse.IsCursorVisible = true;
        alkuValikko.AddItemHandler(0, AloitaPeli);
        alkuValikko.AddItemHandler(1, Exit);

    }
}



