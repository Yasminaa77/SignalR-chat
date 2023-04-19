import { useState, useEffect } from "react";
import {
    HubConnection,
    HubConnectionBuilder,
    LogLevel,
  } from "@microsoft/signalr";
  

  export default function useSignalR(url: string) {
    let [connection, setConnection] = useState<HubConnection | undefined>(
      undefined
    );
  
    useEffect(() => {
      //cancel everything/connection if the component unmount
      let canceld = false;
      //build the connection to the signalR server
      const connection = new HubConnectionBuilder()
        .withUrl(url)
        .withAutomaticReconnect()
        .configureLogging(LogLevel.Information)
        .build();
  
        //try to start the connection
      connection
        .start()
        .then(() => {
          if (!canceld) {
            setConnection(connection);
          }
        })
        .catch((err) => console.log("Signal error", err));
  
      //hande the connection closing
      connection.onclose((error) => {
        if (canceld) {
          return;
        }
        console.log("Signal closed");
        setConnection(undefined);
      });
  
      //if the connection is lost , it wont close. it will try to reconnect
      //so we need to treat this as a lost connection until `onrecinnected` is called
  
      connection.onreconnecting((error) => {
        if (canceld) {
          return;
        }
        console.log("Signal reconnecting");
        setConnection(undefined);
      });
  
      //connection is back
      connection.onreconnected((error) => {
        if (canceld) {
          return;
        }
        console.log("Signal reconnected");
        setConnection(connection);
      });
  
      //clean up the connection when the component unmount
      return () => {
        canceld = true;
        connection.stop();
      };
    }, []);
    return {connection};
  
  

  }
  
  