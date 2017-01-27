using System;
using System.Collections.Generic;
using Terraria;
using Microsoft.Xna.Framework;

namespace Promethium
{
    public struct Location
    {
        public Location(Point p, byte z)
        {
            Pos = p; Z = z;
        }

        public Point Pos;
        public byte Z;
    }

    public class PathFinder
    {
        internal struct Node
        {
            public float F, G;
            public Point PPos;
            public byte Status, PZ;
            public short Jump;

            public Node UpdateStatus(byte newStatus)
            {
                Node ret = this;
                ret.Status = newStatus;
                return ret;
            }
        }

        private Dictionary<Point, List<Node>> nodes;
        private Stack<Point> touchedLocations;

        public bool Debug;
        private PriorityQueue<Location> mOpen;
        private List<Point> mClose;
        private bool mStop;
        private bool mStopped = true;
        private bool mDiagonals = true;
        public int HEstimate = 2;
        public int SearchLimit = 4000;
        private byte mOpenNodeValue = 1;
        private byte mCloseNodeValue = 2;
        
        private Location mLocation;
        private Point mNewLocation;
        private Node mNode;
        private int mCloseNodeCounter = 0;
        private bool mFound = false;
        private sbyte[,] mDirection = new sbyte[8, 2] { { 0, -1 }, { 1, 0 }, { 0, 1 }, { -1, 0 }, { 1, -1 }, { 1, 1 }, { -1, 1 }, { -1, -1 } };
        private Point mEndLocation;
        
        public PathFinder()
        {
            nodes = new Dictionary<Point, List<Node>>();
            touchedLocations = new Stack<Point>();
            mClose = new List<Point>();
            mOpen = new PriorityQueue<Location>(new ComparePFNodeMatrix(nodes), 16);
        }
        
        public bool Stopped
        {
            get { return mStopped; }
        }

        public bool Diagonals
        {
            get { return mDiagonals; }
            set
            {
                mDiagonals = value;
                if (mDiagonals) mDirection = new sbyte[8, 2] { { 0, -1 }, { 1, 0 }, { 0, 1 }, { -1, 0 }, { 1, -1 }, { 1, 1 }, { -1, 1 }, { -1, -1 } };
                else mDirection = new sbyte[4, 2] { { 0, -1 }, { 1, 0 }, { 0, 1 }, { -1, 0 } };
            }
        }
        
        public void FindPathStop()
        {
            mStop = true;
        }

        private List<Node> GetNode(Point p)
        {
            if (!nodes.ContainsKey(p)) nodes.Add(p, new List<Node>());
            return nodes[p];
        }

        private static bool CollisionAt(Point p, Entity en)
        {
            return Collision.SolidCollision(p.ToWorldCoordinates(0, 16 - en.height), en.width, en.height);
        }

        public List<Point> FindPath(Entity en, Point end, short jumpHeight, short currJump = 0)
        {
            lock (this)
            {
                while (touchedLocations.Count > 0) nodes[touchedLocations.Pop()].Clear();

                mFound = false;
                mStop = false;
                mStopped = false;
                mCloseNodeCounter = 0;
                mOpenNodeValue += 2;
                mCloseNodeValue += 2;
                mOpen.Clear();

                Point start = en.position.Add(0, en.height - 16).ToTileCoordinates();
                while (Main.tile[start.X, start.Y].IsSolid()) --start.Y;
                mLocation.Pos = start;
                mLocation.Z = 0;
                mEndLocation = end;

                Node firstNode = new Node();
                firstNode.G = 0;
                firstNode.F = HEstimate;
                firstNode.PPos = start;
                firstNode.PZ = 0;
                firstNode.Status = mOpenNodeValue;

                if (Main.tile[start.X, start.Y + 1].IsSolid()) firstNode.Jump = 0;
                else firstNode.Jump = currJump;

                GetNode(mLocation.Pos).Add(firstNode);
                touchedLocations.Push(mLocation.Pos);

                mOpen.Push(mLocation);

                while (mOpen.Count > 0 && !mStop)
                {
                    mLocation = mOpen.Pop();
                    
                    if (GetNode(mLocation.Pos)[mLocation.Z].Status == mCloseNodeValue) continue;

                    int dx = mEndLocation.X - mLocation.Pos.X;
                    int dy = mEndLocation.Y - mLocation.Pos.Y;
                    if (dx * dx + dy * dy <= 6)
                    {
                        end = mLocation.Pos;
                        List<Node> l = nodes[mLocation.Pos];
                        l[mLocation.Z] = l[mLocation.Z].UpdateStatus(mCloseNodeValue);
                        mFound = true;
                        break;
                    }

                    if (mCloseNodeCounter > SearchLimit)
                    {
                        mStopped = true;
                        return null;
                    }
                    
                    for (var i = mDiagonals ? 7 : 3; i >= 0; --i)
                    {
                        mNewLocation = new Point(mLocation.Pos.X + mDirection[i, 0], mLocation.Pos.Y + mDirection[i, 1]);

                        var onGround = false;
                        var atCeiling = false;

                        if (CollisionAt(new Point(mNewLocation.X, mNewLocation.Y), en)) continue;
                        
                        if (Main.tile[mNewLocation.X, mNewLocation.Y + 1].IsSolid()) onGround = true;
                        else if (CollisionAt(new Point(mNewLocation.X, mNewLocation.Y - 1), en)) atCeiling = true;

                        var jumpLength = nodes[mLocation.Pos][mLocation.Z].Jump;
                        short newJumpLength = jumpLength;

                        if (atCeiling)
                        {
                            if (mNewLocation.X != mLocation.Pos.X) newJumpLength = (short)Math.Max(jumpHeight * 2 + 1, jumpLength + 1);
                            else newJumpLength = (short)Math.Max(jumpHeight * 2, jumpLength + 2);
                        }
                        else if (onGround) newJumpLength = 0;
                        else if (mNewLocation.Y < mLocation.Pos.Y)
                        {
                            if (jumpLength < 2) newJumpLength = 3;
                            else if (jumpLength % 2 == 0) newJumpLength = (short)(jumpLength + 2);
                            else newJumpLength = (short)(jumpLength + 1);
                        }
                        else if (mNewLocation.Y > mLocation.Pos.Y)
                        {
                            if (jumpLength % 2 == 0) newJumpLength = (short)Math.Max(jumpHeight * 2, jumpLength + 2);
                            else newJumpLength = (short)Math.Max(jumpHeight * 2, jumpLength + 1);
                        }
                        else if (!onGround && mNewLocation.X != mLocation.Pos.X)
                            newJumpLength = (short)(jumpLength + 1);

                        if (jumpLength >= 0 && jumpLength % 2 != 0 && mLocation.Pos.X != mNewLocation.X) continue;
                        if (jumpLength >= jumpHeight * 2 && mNewLocation.Y < mLocation.Pos.Y) continue;
                        if (newJumpLength >= jumpHeight * 2 + 6 && mNewLocation.X != mLocation.Pos.X && (newJumpLength - (jumpHeight * 2 + 6)) % 8 != 3) continue;

                        float newG = nodes[mLocation.Pos][mLocation.Z].G + 1;// + newJumpLength / 4F;

                        if (GetNode(mNewLocation).Count > 0)
                        {
                            int lowestJump = short.MaxValue;
                            bool couldMoveSideways = false;
                            for (int j = 0; j < nodes[mNewLocation].Count; ++j)
                            {
                                if (nodes[mNewLocation][j].Jump < lowestJump) lowestJump = nodes[mNewLocation][j].Jump;
                                if (nodes[mNewLocation][j].Jump % 2 == 0 && nodes[mNewLocation][j].Jump < jumpHeight * 2 + 6) couldMoveSideways = true;
                            }

                            if (lowestJump <= newJumpLength && (newJumpLength % 2 != 0 || newJumpLength >= jumpHeight * 2 + 6 || couldMoveSideways))
                                continue;
                        }

                        Point dist = new Point(end.X - mNewLocation.X, end.Y - mNewLocation.Y);
                        // Manhattan: mHEstimate * (Math.Abs(dist.X) + Math.Abs(dist.Y));
                        // MaxDXDY: mHEstimate * (Math.Max(Math.Abs(dist.X), Math.Abs(dist.Y)));
                        // DiagonalShortCut:
                        //     var h_diagonal = Math.Min(Math.Abs(dist.X), Math.Abs(dist.Y));
                        //     var h_straight = (Math.Abs(dist.X) + Math.Abs(dist.Y));
                        //     mHEstimate * 2 * h_diagonal + mHEstimate * (h_straight - 2 * h_diagonal);
                        // EuclideanNoSQR: mHEstimate * (float)(dist.X * dist.X + dist.Y * dist.Y);
                        // Custom:
                        //     var dxy = new Point(Math.Abs(dist.X), Math.Abs(dist.Y));
                        //     var Orthogonal = Math.Abs(dxy.X - dxy.Y);
                        //     var Diagonal = Math.Abs(((dxy.X + dxy.Y) - Orthogonal) / 2);
                        //     mHEstimate * (Diagonal + Orthogonal + dxy.X + dxy.Y);

                        // Euclidean:
                        float newH = HEstimate * (float)Math.Sqrt(dist.X * dist.X + dist.Y * dist.Y);

                        Node newNode = new Node();
                        newNode.Jump = newJumpLength;
                        newNode.PPos = mLocation.Pos;
                        newNode.PZ = mLocation.Z;
                        newNode.G = newG;
                        newNode.F = newG + newH;
                        newNode.Status = mOpenNodeValue;

                        if (Debug) Main.dust[Dust.NewDust(mLocation.Pos.ToWorldCoordinates(), 1, 1, Terraria.ID.DustID.Fire)].noGravity = true;

                        if (nodes[mNewLocation].Count == 0) touchedLocations.Push(mNewLocation);

                        nodes[mNewLocation].Add(newNode);
                        mOpen.Push(new Location(mNewLocation, (byte)(nodes[mNewLocation].Count - 1)));
                    }

                    nodes[mLocation.Pos][mLocation.Z] = nodes[mLocation.Pos][mLocation.Z].UpdateStatus(mCloseNodeValue);
                    ++mCloseNodeCounter;
                }

                if (mFound)
                {
                    mClose.Clear();
                    Point pos = end;

                    Node fPrevNodeTmp = new Node();
                    Node fNodeTmp = nodes[end][0];

                    Point fNode = end;
                    Point fPrevNode = end;

                    Point loc = fNodeTmp.PPos;

                    while (fNode.X != fNodeTmp.PPos.X || fNode.Y != fNodeTmp.PPos.Y)
                    {
                        var fNextNodeTmp = nodes[loc][fNodeTmp.PZ];

                        if ((mClose.Count == 0)
                            || (Main.tile[fPrevNode.X, fPrevNode.Y + 1].IsSolid() && Main.tile[fNode.X, fNode.Y + 1].IsTopSolid())
                            || (Main.tile[fNode.X, fNode.Y + 1].IsSolid() && Main.tile[fPrevNode.X, fPrevNode.Y + 1].IsTopSolid())
                            || fNodeTmp.Jump == 3
                            || (fNextNodeTmp.Jump != 0 && fNodeTmp.Jump == 0)                                                                                                       //mark jumps starts
                            || (fNodeTmp.Jump == 0 && fPrevNodeTmp.Jump != 0)                                                                                                       //mark landings
                            || (fNode.Y > mClose[mClose.Count - 1].Y && fNode.Y > fNodeTmp.PPos.Y)
                            || (fNode.Y < mClose[mClose.Count - 1].Y && fNode.Y < fNodeTmp.PPos.Y)
                            || ((CollisionAt(new Point(fNode.X - 1, fNode.Y), en) || CollisionAt(new Point(fNode.X + 1, fNode.Y), en))
                                && fNode.Y != mClose[mClose.Count - 1].Y && fNode.X != mClose[mClose.Count - 1].X))
                            mClose.Add(fNode);

                        fPrevNode = fNode;
                        pos = fNodeTmp.PPos;
                        fPrevNodeTmp = fNodeTmp;
                        fNodeTmp = fNextNodeTmp;
                        loc = fNodeTmp.PPos;
                        fNode = pos;
                    }

                    mClose.Add(fNode);
                    mStopped = true;
                    return mClose;
                }
                mStopped = true;
                return null;
            }
        }
        
        internal class ComparePFNodeMatrix : IComparer<Location>
        {
            Dictionary<Point, List<Node>> mMatrix;

            public ComparePFNodeMatrix(Dictionary<Point, List<Node>> matrix)
            {
                mMatrix = matrix;
            }

            public int Compare(Location a, Location b)
            {
                return mMatrix[a.Pos][a.Z].F.CompareTo(mMatrix[b.Pos][b.Z].F);
            }
        }
    }
}