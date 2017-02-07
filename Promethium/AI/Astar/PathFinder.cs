using System;
using System.Collections.Generic;
using Terraria;
using Microsoft.Xna.Framework;

namespace Promethium.AI.Astar
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
            public byte Status, PZ, Jump;

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

        public LinkedList<Location> FindPath(Entity en, Point end, byte jumpHeight, byte currJump = 0)
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
                    
                    for (int i = mDiagonals ? 7 : 3; i >= 0; --i)
                    {
                        mNewLocation = new Point(mLocation.Pos.X + mDirection[i, 0], mLocation.Pos.Y + mDirection[i, 1]);

                        bool onGround = false, atCeiling = false;

                        if (CollisionAt(new Point(mNewLocation.X, mNewLocation.Y), en)) continue;
                        else if (Main.tile[mNewLocation.X, mNewLocation.Y + 1].IsSolid()) onGround = true;
                        else if (CollisionAt(new Point(mNewLocation.X, mNewLocation.Y - 1), en)) atCeiling = true;

                        byte jump = nodes[mLocation.Pos][mLocation.Z].Jump;
                        byte newJump = jump;

                        if (atCeiling)
                        {
                            if (mNewLocation.X != mLocation.Pos.X) newJump = (byte)Math.Max(jumpHeight * 2 + 1, jump + 1);
                            else newJump = (byte)Math.Max(jumpHeight * 2, jump + 2);
                        }
                        else if (onGround) newJump = 0;
                        else if (mNewLocation.Y < mLocation.Pos.Y)
                        {
                            if (jump < 2) newJump = 3;
                            else if (jump % 2 == 0) newJump = (byte)(jump + 2);
                            else newJump = (byte)(jump + 1);
                        }
                        else if (mNewLocation.Y > mLocation.Pos.Y)
                        {
                            if (jump % 2 == 0) newJump = (byte)Math.Max(jumpHeight * 2, jump + 2);
                            else newJump = (byte)Math.Max(jumpHeight * 2, jump + 1);
                        }
                        else if (!onGround && mNewLocation.X != mLocation.Pos.X)
                            newJump = (byte)(jump + 1);

                        if (jump >= 0 && jump % 2 != 0 && mLocation.Pos.X != mNewLocation.X) continue;
                        if (jump >= jumpHeight * 2 && mNewLocation.Y < mLocation.Pos.Y) continue;
                        if (newJump >= jumpHeight * 2 + 6 && mNewLocation.X != mLocation.Pos.X && (newJump - (jumpHeight * 2 + 6)) % 8 != 3) continue;

                        float newG = nodes[mLocation.Pos][mLocation.Z].G + 1 + 2 * newJump / (float)jumpHeight;
                        
                        if (GetNode(mNewLocation).Count > 0)
                        {
                            int lowestJump = short.MaxValue;
                            bool couldMoveSideways = false;
                            for (int j = 0; j < nodes[mNewLocation].Count; ++j)
                            {
                                if (nodes[mNewLocation][j].Jump < lowestJump) lowestJump = nodes[mNewLocation][j].Jump;
                                if (nodes[mNewLocation][j].Jump % 2 == 0 && nodes[mNewLocation][j].Jump < jumpHeight * 2 + 6) couldMoveSideways = true;
                            }

                            if (lowestJump <= newJump && (newJump % 2 != 0 || newJump >= jumpHeight * 2 + 6 || couldMoveSideways))
                                continue;
                        }

                        Point dist = new Point(end.X - mNewLocation.X, end.Y - mNewLocation.Y);

                        // Manhattan: mHEstimate * (Math.Abs(dist.X) + Math.Abs(dist.Y));
                        // MaxDXDY: mHEstimate * (Math.Max(Math.Abs(dist.X), Math.Abs(dist.Y)));
                        // DiagonalShortCut:
                        //     float h_diagonal = Math.Min(Math.Abs(dist.X), Math.Abs(dist.Y));
                        //     float h_straight = Math.Abs(dist.X) + Math.Abs(dist.Y);
                        //     mHEstimate * 2 * h_diagonal + mHEstimate * (h_straight - 2 * h_diagonal);
                        // Euclidean: mHEstimate * (float)Math.Sqrt(dist.X * dist.X + dist.Y * dist.Y);
                        // EuclideanNoSQR: mHEstimate * (float)(dist.X * dist.X + dist.Y * dist.Y);
                        // Custom:
                        //     Point dxy = new Point(Math.Abs(dist.X), Math.Abs(dist.Y));
                        //     float Orthogonal = Math.Abs(dxy.X - dxy.Y);
                        //     float Diagonal = Math.Abs(((dxy.X + dxy.Y) - Orthogonal) / 2);
                        //     mHEstimate * (Diagonal + Orthogonal + dxy.X + dxy.Y);
                        
                        float newH = HEstimate * (Math.Abs(dist.X) + Math.Abs(dist.Y));

                        Node newNode = new Node();
                        newNode.Jump = newJump;
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
                    LinkedList<Location> ret = new LinkedList<Location>();

                    Node fPrevNodeTmp = new Node();
                    Node fNodeTmp = nodes[end][0];

                    Location fNode = new Location(end, fNodeTmp.Jump);
                    Point fPrevNode = end;

                    while (fNode.Pos.X != fNodeTmp.PPos.X || fNode.Pos.Y != fNodeTmp.PPos.Y)
                    {
                        Node fNextNodeTmp = nodes[fNodeTmp.PPos][fNodeTmp.PZ];

                        if (ret.Count == 0
                            || (Main.tile[fPrevNode.X, fPrevNode.Y + 1].IsSolid() && Main.tile[fNode.Pos.X, fNode.Pos.Y + 1].IsTopSolid())
                            || (Main.tile[fNode.Pos.X, fNode.Pos.Y + 1].IsSolid() && Main.tile[fPrevNode.X, fPrevNode.Y + 1].IsTopSolid())
                    //      || fNodeTmp.Jump == 3
                            || (fNextNodeTmp.Jump != 0 && fNodeTmp.Jump == 0)                                                                                                       
                            || (fNodeTmp.Jump == 0 && fPrevNodeTmp.Jump != 0)                                                                                        
                            || (fNode.Pos.Y > ret.Last.Value.Pos.Y && fNode.Pos.Y > fNodeTmp.PPos.Y)
                            || (fNode.Pos.Y < ret.Last.Value.Pos.Y && fNode.Pos.Y < fNodeTmp.PPos.Y)
                            || ((CollisionAt(new Point(fNode.Pos.X - 1, fNode.Pos.Y), en) || CollisionAt(new Point(fNode.Pos.X + 1, fNode.Pos.Y), en))
                                && fNode.Pos.Y != ret.Last.Value.Pos.Y && fNode.Pos.X != ret.Last.Value.Pos.X))
                            ret.AddLast(fNode);

                        fPrevNode = fNode.Pos;
                        fNode = new Location(fNodeTmp.PPos, fNodeTmp.Jump);
                        fPrevNodeTmp = fNodeTmp;
                        fNodeTmp = fNextNodeTmp;
                    }

                    ret.AddLast(fNode);
                    mStopped = true;
                    return ret;
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