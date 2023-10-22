//Prevents uncharged shots from damaging Monsters
//swapped `oBeam` with `other`in the first line
if (other.chargebeam && (!other.ibeam) && (!other.wbeam) && (!other.pbeam) && (!other.sbeam) && global.missiles == 0 && global.smissiles == 0)
{
    if canbehit
    {
        if (state != 7)
        {
            event_user(0)
            with (other.id)
                event_user(0)
        }
        else
        {
            with (other.id)
                event_user(1)
        }
    }
}
