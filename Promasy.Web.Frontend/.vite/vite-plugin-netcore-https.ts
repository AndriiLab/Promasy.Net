/* eslint-disable node/no-extraneous-import */
import path from "node:path";
import { existsSync, readFileSync, unlinkSync } from "node:fs";
import type { Plugin } from "vite";
import spawnAsync from "@expo/spawn-async";
import { PeerCertificate, TLSSocket } from "tls";

export default function viteAspNetCoreSslPlugin(): Plugin {
  return {
    name: "vite:netcore-https",
    async config(config) {
      const { cert, key } = await readCertificateAsync();
      const https = () => ({
        https: { cert: readFileSync(cert), key: readFileSync(key) },
      });
      return {
        server: https(),
        preview: https(),
      };
    },
  };
}

async function readCertificateAsync() {
  // This script sets up HTTPS for the application using the ASP.NET Core HTTPS certificate
  const baseFolder =
    process.env.APPDATA !== undefined && process.env.APPDATA !== ""
      ? `${ process.env.APPDATA }/ASP.NET/https`
      : `${ process.env.HOME }/.aspnet/https`;

  const certificateArg = process.argv.map(arg => arg.match(/--name=(?<value>.+)/i)).filter(Boolean)[0];
  const certificateName = certificateArg ? certificateArg.groups?.value : process.env.npm_package_name;

  if (!certificateName) {
    console.error("Invalid certificate name. Run this script in the context of an npm/yarn script or pass --name=<<app>> explicitly.");
    process.exit(-1);
  }

  const certFilePath = path.join(baseFolder, `${ certificateName }.pem`);
  const keyFilePath = path.join(baseFolder, `${ certificateName }.key`);

  if (!existsSync(certFilePath) || !existsSync(keyFilePath)) {
    await generateCertsAsync(baseFolder, certFilePath, keyFilePath, certificateName);
  } else {
    let certDate = getCertExpirationDate(certFilePath);
    if (certDate < new Date()) {
      console.log(`Dev Certificate expired on ${ certDate }. Regenerating certificate...`);
      await generateCertsAsync(baseFolder, certFilePath, keyFilePath, certificateName);
      certDate = getCertExpirationDate(certFilePath);
    }
    console.log(`Dev Certificate valid until ${ certDate }`);
  }

  return { cert: certFilePath, key: keyFilePath };
}

function getCertExpirationDate(certFilePath: string): Date {
  const socket = new TLSSocket(null, {
    cert: readFileSync(certFilePath),
  });
  const cert = socket.getCertificate() as PeerCertificate;
  socket.destroy();

  return new Date(cert.valid_to);
}

async function generateCertsAsync(baseFolder: string, certFilePath: string, keyFilePath: string, certificateName: string) {
  if (existsSync(certFilePath)) {
    unlinkSync(certFilePath);
  }
  if (existsSync(keyFilePath)) {
    unlinkSync(keyFilePath);
  }
  await spawnAsync("dotnet", [
    "dev-certs",
    "https",
    "--export-path",
    certFilePath,
    "--format",
    "Pem",
    "--no-password",
  ], { stdio: "inherit" });
  console.log(`dotnet dev-certs generated ${ certificateName }.pem and ${ certificateName }.key in ${ baseFolder }`);
}