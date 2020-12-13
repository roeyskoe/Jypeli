﻿#region MIT License
/*
 * Copyright (c) 2009 University of Jyväskylä, Department of Mathematical
 * Information Technology.
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */
#endregion

using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using Jypeli.Farseer;
using Jypeli.Physics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Jypeli
{
    /// <summary>
    /// Peliolio, joka noudattaa fysiikkamoottorin määräämiä fysiikan lakeja.
    /// Voidaan kuitenkin myös laittaa noudattamaan lakeja valikoidusti.
    /// </summary>
    public partial class PhysicsBody : IPhysicsBody
    {
        /// <summary>
        /// Kappaleen omistajaolio.
        /// </summary>
        public IPhysicsObject Owner { get; internal set; }

        /// <summary>
        /// Jättääkö olio painovoiman huomioimatta.
        /// </summary>
        public bool IgnoresGravity
        {
            get { return Body.IgnoreGravity; }
            set { Body.IgnoreGravity = value; }
        }

        /// <summary>
        /// Jättääkö olio kaikki fysiikkalogiikat (ks. <c>AddPhysicsLogic</c>)
        /// huomiotta. Vaikuttaa esim. painovoimaan, mutta ei törmäyksiin.
        /// </summary>
        public bool IgnoresPhysicsLogics
        {
            get { return Body.IgnoresPhysicsLogics; }
            set { Body.IgnoresPhysicsLogics = value; }
        }

        #region Constructors

        /// <summary>
        /// Luo uuden fysiikkaolion.
        /// </summary>
        /// <param name="width">Leveys.</param>
        /// <param name="height">Korkeus.</param>
        public PhysicsBody( double width, double height, World world )
            : this( width, height, Shape.Rectangle, world )
        {
        }

        /// <summary>
        /// Luo uuden fysiikkaolion.
        /// </summary>
        /// <param name="width">Leveys.</param>
        /// <param name="height">Korkeus.</param>
        /// <param name="shape">Muoto.</param>
        public PhysicsBody( double width, double height, Shape shape, World world)
            : this( width, height, shape, CreatePhysicsShape( shape, new Vector( width, height ) ), world )
        {
        }

        public PhysicsBody( double width, double height, Shape shape, CollisionShapeParameters shapeParameters, World world)
            : this( width, height, shape, CreatePhysicsShape( shape, new Vector( width, height ), shapeParameters ), world )
        {
        }

        /// <summary>
        /// Luo fysiikkaolion, jonka muotona on säde.
        /// </summary>
        /// <param name="raySegment">Säde.</param>
        public PhysicsBody( RaySegment raySegment, World world)
            : this( 1, 1, raySegment, world)
        {
            this._size = Vector.One;
            this._shape = raySegment;
        }

        /// <summary>
        /// Initializes the object with the given physics shape. The size of
        /// the physicsShape must be the one given.
        /// </summary>
        internal PhysicsBody( double width, double height, Shape shape, Shape physicsShape, World world)
        {
            /* 
             Coefficients c = new Coefficients( DefaultCoefficients.Restitution, DefaultCoefficients.StaticFriction, DefaultCoefficients.DynamicFriction );
             Body = new Body( new PhysicsState( ALVector2D.Zero ), physicsShape, DefaultMass, c, new Lifespan() );
             Body.Tag = this;
             Body.Collided += OnCollided;
             Body.Colliding += OnColliding;
            */
            Body = new FarseerPhysics.Dynamics.Body(world, bodyType: BodyType.Dynamic);
            // TODO:...
            Vertices vertices = PolygonTools.CreateRectangle((float)width / 2 * FSConvert.DisplayToSim, (float)height / 2 * FSConvert.DisplayToSim);
            Fixture f = Body.CreateFixture(new FarseerPhysics.Collision.Shapes.PolygonShape(vertices, 0.01f));
            this._size = new Vector( width, height) * FSConvert.DisplayToSim;
            this._shape = shape;
            LinearDamping = 0.99;
        }        

        #endregion

        public void Update( Time time )
        {
            if ( Velocity.Magnitude > MaxVelocity )
                Velocity = Vector.FromLengthAndAngle( MaxVelocity, Velocity.Angle );
            if ( AngularVelocity > MaxAngularVelocity )
                AngularVelocity = MaxVelocity;

            //base.Update( time );
        }

        public void SetCollisionIgnorer( Ignorer ignorer )
        {
            //this.Body.CollisionIgnorer = new CollisionIgnorerAdapter( ignorer );
        }
    }

    /*
    class CollisionIgnorerAdapter : Physics2DDotNet.Ignorers.Ignorer
    {
        private Jypeli.Ignorer innerIgnorer;

        public override bool BothNeeded
        {
            get { return innerIgnorer.BothNeeded; }
        }

        protected override bool CanCollide( Body thisBody, Body otherBody, Physics2DDotNet.Ignorers.Ignorer other )
        {
            var body1 = (Jypeli.PhysicsBody)( thisBody.Tag );
            var body2 = (Jypeli.PhysicsBody)( otherBody.Tag );
            var otherIgnorer = other == null ? null : ( (CollisionIgnorerAdapter)other ).innerIgnorer;

            return innerIgnorer.CanCollide( body1, body2, otherIgnorer );
        }

        public CollisionIgnorerAdapter( Jypeli.Ignorer adaptee )
        {
            this.innerIgnorer = adaptee;
        }
    }*/
}