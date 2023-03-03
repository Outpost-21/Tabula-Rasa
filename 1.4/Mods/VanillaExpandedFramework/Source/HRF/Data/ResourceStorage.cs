using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace HRF
{
    public class ResourceStorage : IExposable, IComparable, IComparable<ResourceStorage>
    {
        private float resourceAmount;

        public int lastChargedTick;

        public IResourceStorage parent;

        public ResourceProperties resourceProperties;
        public ResourceStorage()
        {

        }
        public ResourceStorage(ResourceProperties resourceProperties, IResourceStorage parent)
        {
            this.resourceProperties = resourceProperties;
            this.parent = parent;
        }
        public float ResourceAmount
        {
            get
            {
                return resourceAmount;
            }
            set
            {
                resourceAmount = value;
                if (resourceAmount > ResourceCapacity)
                {
                    resourceAmount = ResourceCapacity;
                }

                if (resourceAmount < 0)
                {
                    resourceAmount = 0;
                }
                parent.GetResourceFor(resourceProperties)?.UpdateResourceData();
            }
        }
        public float ResourceCapacity
        {
            get
            {
                return this.resourceProperties.maxResourceStorageAmount;
            }
        }

        public bool CanGainCapacity(float newCapacity)
        {
            return ResourceCapacity > newCapacity || ResourceCapacity > 0;
        }

        public static ResourceStorage operator +(ResourceStorage a, ResourceStorage b)
        {
            return new ResourceStorage
            {
                resourceAmount = a.resourceAmount + b.resourceAmount
            };
        }
        public static ResourceStorage operator -(ResourceStorage a, ResourceStorage b)
        {
            return new ResourceStorage
            {
                resourceAmount = a.resourceAmount - b.resourceAmount
            };
        }
        public static ResourceStorage operator *(ResourceStorage a, float b)
        {
            return new ResourceStorage
            {
                resourceAmount = (int)(a.resourceAmount * b),
            };
        }
        public static ResourceStorage operator /(ResourceStorage a, float b)
        {
            return new ResourceStorage
            {
                resourceAmount = (int)(a.resourceAmount / b),
            };
        }
        public static bool operator >(ResourceStorage a, ResourceStorage b)
        {
            return a.resourceAmount + a.resourceAmount > b.resourceAmount + b.resourceAmount;
        }
        public static bool operator <(ResourceStorage a, ResourceStorage b)
        {
            return a.resourceAmount + a.resourceAmount < b.resourceAmount + b.resourceAmount;
        }



        public void ExposeData()
        {
            Scribe_Values.Look(ref resourceAmount, "resourceAmount");
            Scribe_Values.Look(ref lastChargedTick, "lastChargedTick");
        }

        public int CompareTo(object obj)
        {
            var other = obj as ResourceStorage;
            if (other != null)
            {
                return CompareTo(other);
            }
            return 0;
        }

        public int CompareTo(ResourceStorage other)
        {
            if (this > other)
            {
                return 1;
            }
            else if (this < other)
            {
                return -1;
            }
            return 0;
        }
    }
}