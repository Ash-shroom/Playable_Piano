using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playable_Piano.ABCParser
{
    internal class Parser
    {
        /*
        L = 1/4 1/8 usw. falls nicht gegeben, aus M abgeleitet
            dafür M ausgerechnet, wenn kleiner als 0.75 dann 1/16 sonst 1/8

        M = 4/4 3/4 usw.
        Q = 120 60  usw. BPM (Laut L)
              Q / 60 => BPS
              60 / BPS => Frames
        K


        ... G,,A,,B,, C,D,E,F,G,A,B, CDEFGAB cdefgab c'd'e'f'g'a'b' c''d''e'' ...

        z = Pause

        bspw. L = 1/16
        Länge A bzw. A1 = L = 1/16
        Länge A2 = doppelt so lange = 1/8
        Länge A3 = dreimal so lange = 1/8 mit Punkt
        usw.

        A/2 = halb so lange = 1/32
        A/4 = viertel so lange = 1/64
        usw.

        A/ = A/2
        A// = A/4
        usw.

        ^A = Ais
        _A = As

        ^^A = doppel Ais (zwei doppelkreuze)
        =A = Natural

        */

        string input;
        int symbolNr;


        public Parser(string fileContent) 
        {
            this.input = fileContent.Replace("\n","");
            symbolNr = 0;
            parseHeader();
        }

        public List<Note> parse()
        {
            List<Note> notes =new List<Note>();

            notes.Concat(parseBody());

            return notes;
        }

        private void parseHeader() 
        {
            char currentSymbol; 
            while (symbolNr < input.Length)
            {
                currentSymbol = input[symbolNr];

            }
        }


        private List<Note> parseBody()
        {
            List<Note> notes = new List<Note>();
            char currentSymbol;
            while (symbolNr < input.Length)
            {
                currentSymbol = input[symbolNr];
            }
            return notes;
        }
    }
}
