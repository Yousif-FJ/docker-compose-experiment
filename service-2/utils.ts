import dns from 'node:dns';
import os from 'node:os';
import psList from 'ps-list';
import { statfs } from 'fs';

export type SystemInformation = {
    Ips: string[],
    Processes: string[],
    Uptime: string,
    FreeSpace: string
}

export async function getSystemInformation() {

    let processes = (await psList()).map(ps => ps.name);

    let ip = await getIp();

    let uptime = secondsToHuman(os.uptime());

    let totalFreeSpace = await getTotalFreeSpace();


    const result : SystemInformation = {
        FreeSpace : totalFreeSpace.toString(),
        Ips : [ip],
        Uptime : uptime,
        Processes : processes
    }

    return result;
}

/**
 * Translates seconds into human readable format of seconds, minutes, hours, days, and years
 * 
 * @param  {number} seconds The number of seconds to be processed
 * @return {string}         The phrase describing the amount of time
 */
function secondsToHuman(seconds: number): string {
    var levels = [
        [Math.floor(seconds / 31536000), 'years'],
        [Math.floor((seconds % 31536000) / 86400), 'days'],
        [Math.floor(((seconds % 31536000) % 86400) / 3600), 'hours'],
        [Math.floor((((seconds % 31536000) % 86400) % 3600) / 60), 'minutes'],
        [(((seconds % 31536000) % 86400) % 3600) % 60, 'seconds'],
    ];
    var returnText = '';

    for (var i = 0, max = levels.length; i < max; i++) {
        if (levels[i][0] === 0) continue;
        returnText += ' ' + levels[i][0] + ' ' + (levels[i][0] === 1 ? (levels[i][1] as string).substring(0, (levels[i][1] as string).length - 1) : levels[i][1]);
    };
    return returnText.trim();
}

function getIp(): Promise<string> {
    return new Promise((resolve, reject) => {
        const options = { family: 4 };
        dns.lookup(os.hostname(), options, (err, addr) => {
            if (err) {
                reject(err);
            } else {
                resolve(addr);
            }
        })
    })
}

function getTotalFreeSpace(): Promise<number> {
    return new Promise((resolve, reject) => {
        statfs('/', (err, stats) => {
            if (err) {
                reject(err);
            }
            resolve(stats.bsize * stats.bfree);
        })
    })   
}
