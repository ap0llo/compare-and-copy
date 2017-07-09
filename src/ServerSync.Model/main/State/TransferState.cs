using System;
using System.Collections.Generic;
using System.Linq;

namespace ServerSync.Model.State
{
    public class TransferState : IEquatable<TransferState>
    {
        HashSet<string> m_TransferLocations;


        public TransferDirection Direction { get; set; }

        public IEnumerable<string> Locations
        {
            get => m_TransferLocations.ToList();
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                m_TransferLocations = new HashSet<string>(value, StringComparer.InvariantCultureIgnoreCase);
            }
        }


        public TransferState() : this(TransferDirection.None)
        {
        }

        public TransferState(TransferDirection direction) : this(direction, Enumerable.Empty<string>())
        {
        }

        public TransferState(TransferDirection direction, IEnumerable<string> transferLocations)
        {
            if(transferLocations == null)
                throw new ArgumentNullException(nameof(transferLocations));

            Direction = direction;

            if (Flags.EnabledExtendedTransferState)
            {
                m_TransferLocations = new HashSet<string>(transferLocations, StringComparer.InvariantCultureIgnoreCase);                
            }
        }


        public void AddTransferLocation(string path)
        {
            if (Flags.EnabledExtendedTransferState)
            {
                m_TransferLocations.Add(path.Trim());                            
            }
        }

        public void RemoveTransferLocation(string path)
        {
            if (Flags.EnabledExtendedTransferState)
            {
                m_TransferLocations.Remove(path.Trim());                
            }
        }

        public override int GetHashCode()
        {
            int hash = Direction.GetHashCode();

            if (Flags.EnabledExtendedTransferState)
            {                
                foreach (var item in Locations)
                {
                    hash |= item.GetHashCode();                
                }
            }

            return hash;
        }

        public override bool Equals(object obj) => Equals(obj as TransferState);

        public bool Equals(TransferState other)
        {
            if(other == null)
                return false;


            if (Flags.EnabledExtendedTransferState)
            {
                return other.Direction == this.Direction &&
                       !other.Locations.Except(this.Locations, StringComparer.InvariantCultureIgnoreCase).Any();
            }
            else
            {
                return other.Direction == this.Direction;
            }
        }
    }
}
