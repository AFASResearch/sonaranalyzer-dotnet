﻿// Copyright © 2011 - Present RealDimensions Software, LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// Noncompliant: ;
using System;

// Noncompliant: ;

// Noncompliant: {

// Noncompliant: }

// foo ; {} bar

// ; {} foo

// Noncompliant: ++

// Noncompliant: for    ( .. i != 5

// Noncompliant: if ( 1==2

// Noncompliant: while( i > 5

// Noncompliant: catch(

// Noncompliant: switch(

// Noncompliant: try{

// Noncompliant: else{

// &&
// ||
// && &&
// && ||

// Noncompliant: && && &&

// Noncompliant: || || ||

// Noncompliant: || && ||

/*

    hello

    Noncompliant: ;

    ; world

    || && ||

    ;

*/

// Noncompliant: Console.WriteLine("Hello, world!");
// Console.WriteLine("Hello, world!");
// Console.WriteLine("Hello, world!");
//
// Noncompliant: Console.WriteLine("Hello, world!");

/* Noncompliant: Console.WriteLine(); */

namespace Tests.Diagnostics
{


    /// <summary>
    /// ...
    /// </summary>
    /// <code>
    /// Console.WriteLine("Hello, world!");
    /// </code>
    public class CommentedOutCode
    {
        public void M() {
            /* foo */ M();
            M(); /* foo */
        }


        int a; // Noncompliant: Console.WriteLine();
        int b; // Noncompliant: Console.WriteLine();

        // this should be compliant:
        // does *not* overwrite file if (still) exists
    }
}
