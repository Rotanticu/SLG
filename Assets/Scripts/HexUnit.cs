using maho;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
namespace maho
{
    public class HexUnit : MonoBehaviour
    {
        public static HexUnit unitPrefab;
        List<HexCell> pathToTravel;
        const float travelSpeed = 4f;
        public bool moved = false;
        public HexCell Location
        {
            get
            {
                return location;
            }
            set
            {
                if (location)
                {
                    location.FleetUnit = null;
                }
                location = value;
                value.FleetUnit = (Fleet)this;
                //transform.localPosition = value.Position;
                Vector3 y = value.Position;
                y.y = 2;
                transform.localPosition = y;
            }
        }
        void OnEnable()
        {
            if (location)
            {
                transform.localPosition = location.Position;
            }
        }
        public float Orientation
        {
            get
            {
                return orientation;
            }
            set
            {
                orientation = value;
                //transform.localRotation = Quaternion.Euler(0f, value, 0f);

            }
        }

        float orientation;

        public void travel(List<HexCell> path)
        {
            Location = path[path.Count - 1];
            pathToTravel = path;
            StopAllCoroutines();
            StartCoroutine(TravelPath());
        }
        IEnumerator TravelPath()
        {
            Vector3 a, b, c = pathToTravel[0].Position;
            a.y = 2;
            b.y = 2;
            c.y = 2;
            transform.localPosition = c;

            float t = Time.deltaTime * travelSpeed;
            for (int i = 1; i < pathToTravel.Count; i++)
            {
                a = c;
                b = pathToTravel[i - 1].Position;
                b.y = 2;
                c = (b + pathToTravel[i].Position) * 0.5f;
                c.y = 2;
                for (; t < 1f; t += Time.deltaTime * travelSpeed)
                {
                    transform.localPosition = Bezier.GetPoint(a, b, c, t);
                    //Vector3 d = Bezier.GetDerivative(a, b, c, t);
                    //d.y = 0f;
                    //transform.localRotation = Quaternion.LookRotation(d);
                    yield return null;
                }
                t -= 1f;
            }

            a = c;
            b = pathToTravel[pathToTravel.Count - 1].Position;
            b.y = 2;
            c = b;
            for (; t < 1f; t += Time.deltaTime * travelSpeed)
            {
                transform.localPosition = Bezier.GetPoint(a, b, c, t);
                //Vector3 d = Bezier.GetDerivative(a, b, c, t);
                //d.y = 0f;
                //transform.localRotation = Quaternion.LookRotation(d);
                yield return null;
            }
            Vector3 d = location.Position;
            d.y = 2;
            transform.localPosition = d;
            orientation = transform.localRotation.eulerAngles.y;
            ListPool<HexCell>.Add(pathToTravel);
            pathToTravel = null;
        }

        public void Die()
        {
            location.FleetUnit = null;
            Destroy(gameObject);
        }
        public HexCell location;
        public static class Bezier
        {

            public static Vector3 GetPoint(Vector3 a, Vector3 b, Vector3 c, float t)
            {
                float r = 1f - t;
                return r * r * a + 2f * r * t * b + t * t * c;
            }

            public static Vector3 GetDerivative(Vector3 a, Vector3 b, Vector3 c, float t
)
            {
                return 2f * ((1f - t) * (b - a) + t * (c - b));
            }
        }
    }
}
